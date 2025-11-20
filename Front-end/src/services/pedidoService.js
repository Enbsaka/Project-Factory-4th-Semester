// src/services/pedidoService.js
import api from "./api"; // usa o axios já configurado

const getAll = async ({ pagina = 1, itensPorPagina = 10 } = {}) => {
  try {
    const response = await api.get(`/Pedido?pagina=${pagina}&tamanhoPagina=${itensPorPagina}`);

    // Tratar 204 (No Content) como lista vazia ao invés de erro
    if (response.status === 204) {
      return {
        pedidos: [],
        totalItens: 0,
        paginaAtual: pagina,
        totalPaginas: 0,
      };
    }

    if (response.status !== 200) throw new Error("Erro ao buscar pedidos");

    const data = response.data || {};

    // O backend retorna um objeto Paginador<Pedido>
    const pedidos = data.itens || data.pedidos || [];

    return {
      pedidos,
      totalItens: data.totalItens ?? pedidos.length ?? 0,
      paginaAtual: data.paginaAtual ?? pagina,
      totalPaginas: data.totalPaginas ?? 1,
    };
  } catch (error) {
    // Se não autorizado (ex.: token expirado), retorna conjunto vazio para não quebrar dashboard
    if (error?.response?.status === 401) {
      console.warn("Não autorizado em pedidoService.getAll (401). Retornando vazio.");
      return { pedidos: [], totalItens: 0, paginaAtual: pagina, totalPaginas: 0 };
    }
    console.error("Erro em pedidoService.getAll:", error.response?.data || error.message || error);
    throw error;
  }
};


const getById = async (id) => {
  try {
    const response = await api.get(`/pedido/${id}`);
    if (response.status !== 200) throw new Error("Pedido não encontrado");
    return response.data;
  } catch (error) {
    console.error("Erro em pedidoService.getById:", error.response?.data || error.message || error);
    throw error;
  }
};

const getByCliente = async (clienteId, status) => {
  try {
    const params = {};
    if (status) params.status = status;

    const response = await api.get(`/pedido/cliente/${clienteId}`, { params });

    // Tratar 204 (No Content) como objeto paginado vazio
    if (response.status === 204) {
      return {
        itens: [],
        totalItens: 0,
        paginaAtual: 1,
        totalPaginas: 0,
      };
    }

    if (response.status !== 200) throw new Error("Pedidos não encontrados");
    return response.data;
  } catch (error) {
    console.error("Erro em pedidoService.getByCliente:", error.response?.data || error.message || error);
    throw error;
  }
};

// Para dashboard: receita mensal via API (apenas pedidos finalizados)
const getReceitaMensal = async () => {
  try {
    const { data } = await api.get(`/Pedido/receita-mensal`);
    return Number(data ?? 0);
  } catch (error) {
    console.error("Erro em pedidoService.getReceitaMensal:", error.response?.data || error.message || error);
    return 0;
  }
};

// Atualizar itens do pedido (somente quando status = Carrinho)
const updateItems = async (id, produtos) => {
  try {
    const payload = { produtos: produtos || [] };
    const response = await api.put(`/pedido/${id}`, payload);
    if (response.status !== 204) throw new Error("Falha ao atualizar itens do pedido");
    return true;
  } catch (error) {
    console.error("Erro em pedidoService.updateItems:", error.response?.data || error.message || error);
    throw error;
  }
};

// Atualizar cupom do pedido (enviar como FormData; vazio remove cupom)
const updateCupom = async (id, cupomCodigo) => {
  try {
    const form = new FormData();
    form.append("cupomCodigo", cupomCodigo ?? "");
    const response = await api.patch(`/pedido/${id}/cupom`, form);
    if (response.status !== 204) throw new Error("Falha ao atualizar cupom do pedido");
    return true;
  } catch (error) {
    console.error("Erro em pedidoService.updateCupom:", error.response?.data || error.message || error);
    throw error;
  }
};

// Finalizar pedido (muda status para Finalizado)
const finalize = async (id) => {
  try {
    const response = await api.post(`/pedido/finalizar/${id}`);
    if (response.status !== 204) throw new Error("Falha ao finalizar pedido");
    return true;
  } catch (error) {
    console.error("Erro em pedidoService.finalize:", error.response?.data || error.message || error);
    throw error;
  }
};

// Remover pedido
const remove = async (id) => {
  try {
    const response = await api.delete(`/pedido/${id}`);
    if (response.status !== 204) throw new Error("Falha ao excluir pedido");
    return true;
  } catch (error) {
    console.error("Erro em pedidoService.remove:", error.response?.data || error.message || error);
    throw error;
  }
};

export default {
  getAll,
  getById,
  getByCliente,
  getReceitaMensal,
  updateItems,
  updateCupom,
  finalize,
  remove,
};
