import { ref, onMounted, watch } from "vue";
import { useNotifications } from "./useNotifications.js";
import productService from "../../../services/productService.js";
import api from "../../../services/api.js";

export function useProducts() {
  const produtos = ref([]);
  const categorias = ref([]);
  const carregando = ref(false);
  const erro = ref(null);

  const busca = ref("");
  const filtroCategorias = ref([]);
  const paginaAtual = ref(1);
  const totalPaginas = ref(1);
  const itensPorPagina = ref(6);

  const modalEditarAberto = ref(false);
  const modalExcluirAberto = ref(false);
  const modalVariacoesAberto = ref(false);
  const modalNovoAberto = ref(false);
  const modalDetalhesAberto = ref(false);
  const produtoSelecionado = ref(null);

  const variacoes = ref([]);
  const novaVariacao = ref({
    nome: "",
    preco: "",
    cor: "",
    tamanho: "",
    imagem: null,
  });

  const { notificacao, show } = useNotifications();

  async function carregarProdutos() {
    try {
      carregando.value = true;
      erro.value = null;

      const { produtos: itens, paginaAtual: pag, totalPaginas: total } =
        await productService.getAll({
          nome: busca.value,
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


  async function salvarProduto(id, formData) {
    try {
      await productService.update(id, formData);
      modalEditarAberto.value = false;
      await carregarProdutos();
      show("Produto atualizado com sucesso!", "success");
    } catch (err) {
      console.error("Erro ao editar produto:", err);
      const mensagem = err?.response?.data || "Erro ao editar produto.";
      show(mensagem, "error");
    }
  }

  async function excluirProduto(id) {
    try {
      await productService.remove(id);
      modalExcluirAberto.value = false;
      await carregarProdutos();
      show("Produto excluído com sucesso!", "success");
    } catch (e) {
      console.error(e);
      const mensagem = e?.response?.data || "Erro ao excluir produto.";
      show(mensagem, "error");
    }
  }


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


  onMounted(() => {
    carregarProdutos();
    carregarCategorias();
  });

  watch([busca, filtroCategorias], () => {
    paginaAtual.value = 1;
    carregarProdutos();
  });

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
