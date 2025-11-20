import { ref, onMounted, watch } from "vue";
import { useNotifications } from "./useNotifications.js";
import productService from "../../../services/productService.js";
import api from "../../../services/api.js";

/**
 * Composable responsável pela gestão de produtos:
 * - Listagem e paginação
 * - Filtros e busca
 * - CRUD (criar, editar, excluir)
 * - Variações
 * - Controle dos modais
 */
export function useProducts() {
  // --- Estado principal ---
  const produtos = ref([]);
  const categorias = ref([]);
  const carregando = ref(false);
  const erro = ref(null);

  // --- Filtros e paginação ---
  const busca = ref("");
  const filtroCategorias = ref([]);
  const paginaAtual = ref(1);
  const totalPaginas = ref(1);
  const itensPorPagina = ref(6);

  // --- Modais e seleção ---
  const modalEditarAberto = ref(false);
  const modalExcluirAberto = ref(false);
  const modalVariacoesAberto = ref(false);
  const modalNovoAberto = ref(false);
  const modalDetalhesAberto = ref(false);
  const produtoSelecionado = ref(null);

  // --- Variações ---
  const variacoes = ref([]);
  const novaVariacao = ref({
    nome: "",
    preco: "",
    cor: "",
    tamanho: "",
    imagem: null,
  });

  // --- Notificações ---
  const { notificacao, show } = useNotifications();

  // =====================================================
  // 🔹 Funções principais
  // =====================================================

  async function carregarProdutos() {
    try {
      carregando.value = true;
      erro.value = null;

      const { produtos: itens, paginaAtual: pag, totalPaginas: total } =
        await productService.getAll({
          nome: busca.value,
          // Envia múltiplos IDs de categorias selecionadas (subcategorias e/ou pai)
          categoriaIds: Array.isArray(filtroCategorias.value) && filtroCategorias.value.length > 0
            ? filtroCategorias.value
            : undefined,
          pagina: paginaAtual.value,
          itensPorPagina: itensPorPagina.value,
        });

      produtos.value = itens.map((p) => ({
        id: p.id,
        nome: p.nome,
        descricao: p.descricao,
        preco: p.preco ? `R$ ${Number(p.preco).toFixed(2)}` : "R$ 0,00",
        precoNumber: p.preco || 0,
        codigoDeBarra: p.codigoDeBarra,
        cor: p.cor,
        tamanho: p.tamanho,
        categoria: p.categoriaNome || (p.categoria ? p.categoria.nome : "Sem categoria"),
        categoriaId: p.categoriaId,
        produtoPaiId: p.produtoPaiId,
        imagem: p.imagemURL || "https://via.placeholder.com/50",
        variacoes:
          p.variacoes?.map((v) => ({
            id: v.id,
            nome: v.nome,
            preco: v.preco,
            cor: v.cor,
            tamanho: v.tamanho,
            imagem: v.imagemURL || "https://via.placeholder.com/50",
          })) || [],
      }));

      paginaAtual.value = pag || 1;
      totalPaginas.value = total || 1;
    } catch (e) {
      erro.value = "Erro ao carregar produtos.";
      console.error(e);
    } finally {
      carregando.value = false;
    }
  }

  async function carregarCategorias() {
    try {
      // Busca árvore de categorias para dropdown hierárquico
      const { data } = await api.get("/categoria/hierarquia");
      categorias.value = data;
    } catch (e) {
      console.error("Erro ao carregar categorias", e);
    }
  }

  function mudarPagina(novaPagina) {
    if (novaPagina < 1 || novaPagina > totalPaginas.value) return;
    paginaAtual.value = novaPagina;
    carregarProdutos();
  }

  function mudarItensPorPagina(novoTamanho) {
    const n = Number(novoTamanho) || 10;
    itensPorPagina.value = n;
    paginaAtual.value = 1;
    carregarProdutos();
  }

  // =====================================================
  // 🔹 CRUD
  // =====================================================

  async function salvarProduto(id, formData) {
    try {
      await productService.update(id, formData);
      modalEditarAberto.value = false;
      await carregarProdutos();
      show("Produto atualizado com sucesso!", "success");
    } catch (err) {
      console.error("Erro ao editar produto:", err);
      show("Erro ao editar produto.", "error");
    }
  }

  async function excluirProduto(id) {
    try {
      await productService.remove(id);
      await carregarProdutos();
      show("Produto excluído com sucesso!", "success");
    } catch (e) {
      console.error(e);
      show("Erro ao excluir produto.", "error");
    }
  }

  // =====================================================
  // 🔹 Variações
  // =====================================================

  async function carregarVariacoes(produtoPaiId) {
    try {
      const { produtos: itens } = await productService.getAll({ pagina: 1, itensPorPagina: 50 });
      variacoes.value = itens
        .filter((p) => p.produtoPaiId === produtoPaiId)
        .map((p) => ({
          id: p.id,
          nome: p.nome,
          preco: p.preco ? `R$ ${Number(p.preco).toFixed(2)}` : "R$ 0,00",
          cor: p.cor,
          tamanho: p.tamanho,
          imagem: p.imagemURL || "https://via.placeholder.com/50",
        }));
    } catch (e) {
      console.error(e);
    }
  }

  async function criarVariacao() {
    try {
      if (!produtoSelecionado.value || !produtoSelecionado.value.id) {
        console.error('Produto não selecionado para criar variação.');
        show("Selecione um produto para adicionar variações.", "error");
        return;
      }
      const formData = new FormData();
      Object.entries(novaVariacao.value).forEach(([k, v]) => {
        if (v !== null) formData.append(k, v);
      });
      formData.append("produtoPaiId", produtoSelecionado.value.id);
      await productService.create(formData);
      novaVariacao.value = { nome: "", preco: "", cor: "", tamanho: "", imagem: null };
      await carregarVariacoes(produtoSelecionado.value.id);
      await carregarProdutos();
      show("Variação criada com sucesso!", "success");
    } catch (e) {
      console.error(e);
      show("Erro ao criar variação.", "error");
    }
  }

  // =====================================================
  // 🔹 Controle dos modais
  // =====================================================

  function abrirModalEdicao(produto) {
    produtoSelecionado.value = {
      id: produto.id,
      nome: produto.nome,
      descricao: produto.descricao,
      preco: produto.precoNumber ?? produto.preco,
      imagem: produto.imagem,
      categoriaId: produto.categoriaId ?? null,
    };
    modalEditarAberto.value = true;
  }

  function abrirModalVariacoes(produto) {
    produtoSelecionado.value = { ...produto };
    carregarVariacoes(produto.id);
    modalVariacoesAberto.value = true;
  }

  function abrirModalDetalhes(produto) {
    produtoSelecionado.value = { ...produto };
    modalDetalhesAberto.value = true;
  }

  function abrirModalExcluir(produto) {
    produtoSelecionado.value = { ...produto };
    modalExcluirAberto.value = true;
  }

  function abrirModalNovoProduto() {
    modalNovoAberto.value = true;
  }

  // (notificações centralizadas via useNotifications)

  // =====================================================
  // 🔹 Lifecycle
  // =====================================================

  onMounted(() => {
    carregarProdutos();
    carregarCategorias();
  });

  // Recarrega ao alterar busca ou filtros
  watch([busca, filtroCategorias], () => {
    paginaAtual.value = 1;
    carregarProdutos();
  });

  // =====================================================
  // 🔹 Retorno do composable
  // =====================================================

  return {
    produtos,
    categorias,
    carregando,
    erro,
    busca,
    filtroCategorias,
    paginaAtual,
    totalPaginas,
    itensPorPagina,

    modalEditarAberto,
    modalExcluirAberto,
    modalVariacoesAberto,
    modalNovoAberto,
    modalDetalhesAberto,
    produtoSelecionado,
    variacoes,

    abrirModalEdicao,
    abrirModalVariacoes,
    abrirModalDetalhes,
    abrirModalExcluir,
    abrirModalNovoProduto,

    salvarProduto,
    excluirProduto,
    criarVariacao,
    carregarProdutos,
    mudarPagina,
    mudarItensPorPagina,

    notificacao,
  };
}
