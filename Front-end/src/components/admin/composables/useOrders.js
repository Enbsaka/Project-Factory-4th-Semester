import { ref, computed, onMounted } from "vue";
import pedidoService from "../../../services/pedidoService.js";
import { useNotifications } from "./useNotifications.js";

export function useOrders() {
  // Estado principal
  const pedidos = ref([]);
  const carregando = ref(false);
  const erro = ref(null);

  // Filtros
  const busca = ref("");
  const filtroStatus = ref(""); // "" | 0 | 1
  const dataInicio = ref("");
  const dataFim = ref("");

  // Paginação
  const paginaAtual = ref(1);
  const totalPaginas = ref(1);
  const itensPorPagina = ref(10);

  // Modais e seleção
  const modalEditarAberto = ref(false);
  const modalExcluirAberto = ref(false);
  const pedidoSelecionado = ref(null);

  // Notificações
  const { notificacao, show } = useNotifications();

  async function carregarPedidos() {
    carregando.value = true;
    erro.value = null;
    try {
      const res = await pedidoService.getAll({ pagina: 1, itensPorPagina: 100 });
      pedidos.value = res.pedidos || [];
    } catch (e) {
      erro.value = e;
      console.error("Erro ao carregar pedidos:", e);
      show("Erro ao carregar pedidos.", "error");
    } finally {
      carregando.value = false;
    }
  }

  onMounted(() => carregarPedidos());

  const pedidosFiltrados = computed(() => {
    const buscaStr = (busca.value || "").toLowerCase();
    const statusNum = filtroStatus.value !== "" ? Number(filtroStatus.value) : null;

    const inicio = dataInicio.value ? new Date(`${dataInicio.value}T00:00:00`) : null;
    const fim = dataFim.value ? new Date(`${dataFim.value}T23:59:59.999`) : null;

    const filtrados = (pedidos.value || []).filter((p) => {
      const nome = p?.cliente?.nome?.toLowerCase?.() || "";
      const idStr = String(p?.id || "").toLowerCase();
      const okBusca = nome.includes(buscaStr) || idStr.includes(buscaStr);
      const okStatus = statusNum === null || p.status === statusNum;
      const dataRaw = p?.dataPedido ?? p?.DataPedido ?? null;
      const data = dataRaw ? new Date(dataRaw) : null;
      const okData = !inicio && !fim
        ? true
        : data
          ? (!inicio || data >= inicio) && (!fim || data <= fim)
          : false;
      return okBusca && okStatus && okData;
    });

    totalPaginas.value = Math.max(1, Math.ceil(filtrados.length / itensPorPagina.value));
    if (paginaAtual.value > totalPaginas.value) paginaAtual.value = totalPaginas.value;
    const start = (paginaAtual.value - 1) * itensPorPagina.value;
    return filtrados.slice(start, start + itensPorPagina.value);
  });

  // Mantido por compatibilidade caso necessário em outras telas
  const pedidosPaginados = computed(() => pedidosFiltrados.value);

  function mudarPagina(novaPagina) {
    if (novaPagina < 1 || novaPagina > totalPaginas.value) return;
    paginaAtual.value = novaPagina;
  }

  function mudarItensPorPagina(novoTamanho) {
    const n = Number(novoTamanho) || 10;
    itensPorPagina.value = n;
    paginaAtual.value = 1;
  }

  // Ações
  function abrirModalEditar(pedido) {
    pedidoSelecionado.value = pedido;
    modalEditarAberto.value = true;
  }

  function abrirModalExcluir(pedido) {
    pedidoSelecionado.value = pedido;
    modalExcluirAberto.value = true;
  }

  async function salvarCupom(cupomCodigo) {
    const id = pedidoSelecionado.value?.id;
    if (!id) return;
    try {
      await pedidoService.updateCupom(id, cupomCodigo);
      show(cupomCodigo ? "Cupom aplicado com sucesso!" : "Cupom removido com sucesso!", "success");
      modalEditarAberto.value = false;
      await carregarPedidos();
    } catch (e) {
      const msg = e?.response?.data || e?.message || "Erro ao atualizar cupom";
      show(String(msg), "error");
    }
  }

  async function finalizarPedido() {
    const id = pedidoSelecionado.value?.id;
    if (!id) return;
    try {
      await pedidoService.finalize(id);
      show("Pedido finalizado com sucesso!", "success");
      modalEditarAberto.value = false;
      await carregarPedidos();
    } catch (e) {
      const msg = e?.response?.data || e?.message || "Erro ao finalizar pedido";
      show(String(msg), "error");
    }
  }

  async function excluirPedido() {
    const id = pedidoSelecionado.value?.id;
    if (!id) return;
    try {
      await pedidoService.remove(id);
      show("Pedido excluído com sucesso!", "success");
      modalExcluirAberto.value = false;
      await carregarPedidos();
    } catch (e) {
      const msg = e?.response?.data || e?.message || "Erro ao excluir pedido";
      show(String(msg), "error");
    }
  }

  return {
    // estado
    pedidos,
    carregando,
    erro,
    // filtros
    busca,
    filtroStatus,
    dataInicio,
    dataFim,
    pedidosFiltrados,
    pedidosPaginados,
    paginaAtual,
    totalPaginas,
    itensPorPagina,
    // modais
    modalEditarAberto,
    modalExcluirAberto,
    pedidoSelecionado,
    // notificações
    notificacao,
    // métodos
    carregarPedidos,
    abrirModalEditar,
    abrirModalExcluir,
    salvarCupom,
    finalizarPedido,
    excluirPedido,
    mudarPagina,
    mudarItensPorPagina,
  };
}