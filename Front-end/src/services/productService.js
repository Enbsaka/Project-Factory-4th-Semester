// src/services/productService.js
import api from "./api";
const RESOURCE = "/produto";

async function getAll({
  nome,
  cor,
  tamanho,
  categoria,
  categoriaId,
  categoriaIds,
  produtoPaiId,
  pagina = 1,
  itensPorPagina = 10,
} = {}) {
  try {
    // Usa URLSearchParams para garantir múltiplos categoriaIds como chaves repetidas
    const params = new URLSearchParams();
    if (nome) params.set("nome", nome);
    if (cor) params.set("cor", cor);
    if (tamanho) params.set("tamanho", tamanho);
    if (categoria) params.set("categoria", categoria);
    if (categoriaId) params.set("categoriaId", categoriaId);
    if (produtoPaiId) params.set("produtoPaiId", produtoPaiId);
    // Alinha com o backend que espera 'paginaAtual' e 'tamanhoPagina'
    params.set("paginaAtual", String(pagina));
    params.set("tamanhoPagina", String(itensPorPagina));

    if (Array.isArray(categoriaIds) && categoriaIds.length > 0) {
      categoriaIds.forEach((id) => params.append("categoriaIds", id));
    }

    // Monta URL com query string manualmente para evitar serialização incorreta
    const url = `${RESOURCE}?${params.toString()}`;
    const { data } = await api.get(url);

    // Normaliza propriedades vindas do backend (PascalCase) e camelCase
    const itens = data?.itens ?? data?.Itens ?? [];
    const paginaAtualResp = data?.paginaAtual ?? data?.PaginaAtual ?? 1;
    const totalPaginasResp = data?.totalPaginas ?? data?.TotalPaginas ?? 1;
    const totalItensResp = data?.totalItens ?? data?.TotalItens ?? itens.length ?? 0;

    return {
      produtos: itens,
      paginaAtual: paginaAtualResp,
      totalPaginas: totalPaginasResp,
      totalItens: totalItensResp,
    };
  } catch (error) {
    console.error("Erro em productService.getAll:", error);
    throw error;
  }
}

async function getById(id) {
  const { data } = await api.get(`${RESOURCE}/${id}`);
  return data;
}

async function create(formData) {
  const { data } = await api.post(RESOURCE, formData);
  return data;
}

async function update(id, data) {
  if (!id) throw new Error("ID do produto é obrigatório para atualizar!");
  const { data: resp } = await api.patch(`${RESOURCE}/${id}`, data);
  return resp;
}

async function remove(id) {
  const { data } = await api.delete(`${RESOURCE}/${id}`);
  return data;
}

export default {
  getAll,
  getById,
  create,
  update,
  remove,
};
