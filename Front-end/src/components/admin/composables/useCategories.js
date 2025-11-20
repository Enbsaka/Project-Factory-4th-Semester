import { ref, computed, onMounted } from "vue";
import { useNotifications } from "./useNotifications.js";
import categoriaService from "../../../services/categoriaService.js";
import api from "../../../services/api.js";

// Composable para gestão de categorias no admin: listagem, busca, filtros e paginação
export function useCategories() {
  // Estado
  const categoriasTree = ref([]); // árvore para dropdown
  const categoriasRaw = ref([]); // lista simples do endpoint /categoria
  const carregando = ref(false);
  const erro = ref(null);

  // Filtros e paginação
  const busca = ref("");
  const filtroCategorias = ref([]); // lista de IDs (pais e/ou subs)
  const mostrarApenasPais = ref(false);
  const mostrarApenasSubs = ref(false);
  const paginaAtual = ref(1);
  const itensPorPagina = ref(10);

  // Seleção e modais
  const categoriaSelecionada = ref(null);
  const modalEditarAberto = ref(false);
  const modalExcluirAberto = ref(false);
  const modalNovaSubAberto = ref(false);
  // Notificações
  const { notificacao, show } = useNotifications();

  // Helpers
  function normalizarCategoria(c) {
    const id = c.id ?? c.Id;
    const nome = c.nome ?? c.Nome;
    const paiId = c.categoriaPaiId ?? c.CategoriaPaiId ?? null;
    return { id, nome, categoriaPaiId: paiId };
  }

  // Carrega árvore (para filtros) e lista plana (para tabela)
  async function carregarCategorias() {
    try {
      carregando.value = true;
      erro.value = null;

      try {
        const { data } = await api.get("/categoria/hierarquia");
        categoriasTree.value = Array.isArray(data) ? data : [];
      } catch (e) {
        console.warn("Falha ao carregar hierarquia de categorias", e);
        categoriasTree.value = [];
      }

      categoriasRaw.value = await categoriaService.getAll();
    } catch (e) {
      console.error(e);
      erro.value = "Erro ao carregar categorias.";
    } finally {
      carregando.value = false;
    }
  }

  // Mapa de nome do pai para exibição
  const nomePaiPorId = computed(() => {
    const map = new Map();
    // da árvore
    function caminhar(nodes, paiId = null) {
      for (const n of nodes || []) {
        map.set(n.id ?? n.Id, paiId);
        caminhar(n.subcategorias || [], n.id ?? n.Id);
      }
    }
    caminhar(categoriasTree.value);
    // completa com flat
    categoriasRaw.value.forEach((c) => {
      map.set(c.id, c.categoriaPaiId ?? null);
    });
    return map;
  });

  const nomePorId = computed(() => {
    const map = new Map();
    function caminhar(nodes) {
      for (const n of nodes || []) {
        const id = n.id ?? n.Id;
        const nome = n.nome ?? n.Nome;
        map.set(id, nome);
        caminhar(n.subcategorias || []);
      }
    }
    caminhar(categoriasTree.value);
    categoriasRaw.value.forEach((c) => map.set(c.id, c.nomeCategoria));
    return map;
  });

  // Flatten para tabela, com contagem de produtos da árvore
  function contarProdutos(node) {
    let total = (node.produtos?.length ?? 0);
    (node.subcategorias || []).forEach((c) => {
      total += contarProdutos(c);
    });
    return total;
  }

  const produtosPorCategoriaId = computed(() => {
    const map = new Map();
    function caminhar(nodes) {
      for (const n of nodes || []) {
        const id = n.id ?? n.Id;
        map.set(id, contarProdutos(n));
        caminhar(n.subcategorias || []);
      }
    }
    caminhar(categoriasTree.value);
    return map;
  });

  // Busca nó na árvore por id
  function encontrarNoPorId(id, nodes = categoriasTree.value) {
    for (const n of nodes || []) {
      const nid = n.id ?? n.Id;
      if (String(nid) === String(id)) return n;
      const encontrado = encontrarNoPorId(id, n.subcategorias || []);
      if (encontrado) return encontrado;
    }
    return null;
  }

  const categoriasTabela = computed(() => {
    // base flat
    const base = categoriasRaw.value.map((c) => ({
      id: c.id,
      nome: c.nomeCategoria,
      categoriaPaiId: c.categoriaPaiId ?? nomePaiPorId.value.get(c.id) ?? null,
      paiNome: nomePorId.value.get(c.categoriaPaiId ?? nomePaiPorId.value.get(c.id) ?? null) || "—",
      tipo: (c.categoriaPaiId ?? nomePaiPorId.value.get(c.id)) ? "Sub Categoria" : "Categoria Pai",
      totalProdutos: produtosPorCategoriaId.value.get(c.id) ?? c.totalProdutos ?? 0,
    }));

    // filtros
    let filtradas = base;
    if (busca.value?.trim()) {
      const termo = busca.value.trim().toLowerCase();
      filtradas = filtradas.filter((c) => (c.nome || "").toLowerCase().includes(termo));
    }
    if (Array.isArray(filtroCategorias.value) && filtroCategorias.value.length > 0) {
      const set = new Set(filtroCategorias.value.map((x) => String(x)));
      filtradas = filtradas.filter((c) => set.has(String(c.id)));
    }
    if (mostrarApenasPais.value && !mostrarApenasSubs.value) {
      filtradas = filtradas.filter((c) => !c.categoriaPaiId);
    }
    if (mostrarApenasSubs.value && !mostrarApenasPais.value) {
      filtradas = filtradas.filter((c) => !!c.categoriaPaiId);
    }

    // paginação
    const total = filtradas.length;
    const totalPaginas = Math.max(1, Math.ceil(total / itensPorPagina.value));
    if (paginaAtual.value > totalPaginas) paginaAtual.value = totalPaginas;
    const inicio = (paginaAtual.value - 1) * itensPorPagina.value;
    const paginaItens = filtradas.slice(inicio, inicio + itensPorPagina.value);

    return {
      itens: paginaItens,
      total,
      totalPaginas,
    };
  });

  function mudarPagina(novaPagina) {
    const totalPaginas = categoriasTabela.value.totalPaginas;
    if (novaPagina < 1 || novaPagina > totalPaginas) return;
    paginaAtual.value = novaPagina;
  }

  function mudarItensPorPagina(novoTamanho) {
    const n = Number(novoTamanho) || 10;
    itensPorPagina.value = n;
    paginaAtual.value = 1;
  }

  // Ações
  async function salvarCategoria(id, { nome, categoriaPaiId }) {
    try {
      await categoriaService.update(id, { nome, categoriaPaiId });
      modalEditarAberto.value = false;
      await carregarCategorias();
      show("Categoria atualizada com sucesso!", "success");
    } catch (e) {
      const status = e?.response?.status;
      if (status === 401 || status === 403) {
        show("Faça login como administrador para editar.", "error");
      } else {
        show("Erro ao atualizar categoria.", "error");
      }
      console.error("Erro ao editar categoria:", e);
    }
  }

  async function excluirCategoria(id) {
    try {
      // Bloqueia exclusão de categorias pai que possuem subcategorias
      const no = encontrarNoPorId(id);
      if (no && Array.isArray(no.subcategorias) && no.subcategorias.length > 0) {
        show("Não é possível excluir uma categoria pai com subcategorias.", "error");
        return;
      }

      await categoriaService.remove(id);
      modalExcluirAberto.value = false;
      await carregarCategorias();
      show("Categoria excluída com sucesso!", "success");
    } catch (e) {
      const status = e?.response?.status;
      if (status === 401 || status === 403) {
        show("Faça login como administrador para excluir.", "error");
      } else {
        show("Erro ao excluir categoria.", "error");
      }
      console.error("Erro ao excluir categoria:", e);
    }
  }

  async function criarSubcategoria(paiId, nome) {
    try {
      await categoriaService.create({ nome, categoriaPaiId: paiId });
      modalNovaSubAberto.value = false;
      await carregarCategorias();
      show("Subcategoria criada com sucesso!", "success");
    } catch (e) {
      const status = e?.response?.status;
      if (status === 401 || status === 403) {
        show("Faça login como administrador para criar.", "error");
      } else {
        show("Erro ao criar subcategoria.", "error");
      }
      console.error("Erro ao criar subcategoria:", e);
    }
  }

  // Controle de modais
  function abrirModalEditar(categoria) {
    categoriaSelecionada.value = { ...categoria };
    modalEditarAberto.value = true;
  }
  function abrirModalExcluir(categoria) {
    categoriaSelecionada.value = { ...categoria };
    modalExcluirAberto.value = true;
  }
  function abrirModalNovaSub(categoria) {
    categoriaSelecionada.value = { ...categoria };
    modalNovaSubAberto.value = true;
  }

  onMounted(() => {
    carregarCategorias();
  });

  return {
    // estado
    categoriasTree,
    categoriasRaw,
    carregando,
    erro,
    // filtros
    busca,
    filtroCategorias,
    mostrarApenasPais,
    mostrarApenasSubs,
    paginaAtual,
    itensPorPagina,
    // tabela
    categoriasTabela,
    mudarPagina,
    mudarItensPorPagina,
    // modais
    categoriaSelecionada,
    modalEditarAberto,
    modalExcluirAberto,
    modalNovaSubAberto,
    abrirModalEditar,
    abrirModalExcluir,
    abrirModalNovaSub,
    // ações
    salvarCategoria,
    excluirCategoria,
    criarSubcategoria,
    notificacao,
    // carregar
    carregarCategorias,
  };
}