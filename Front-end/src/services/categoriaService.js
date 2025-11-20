// src/services/categoriaService.js
import api from "./api"; // usa o axios configurado com baseURL

// Pega todas as categorias
const getAll = async () => {
  try {
    const response = await api.get("/categoria");
    if (response.status !== 200) throw new Error("Erro ao buscar categorias");

    // Normaliza propriedades (backend pode retornar Id/Nome com maiúsculas)
    return (response.data || []).map((c) => {
      const id = c.id ?? c.Id;
      const nome = c.nome ?? c.Nome;
      const total = c.totalProdutos ?? c.TotalProdutos ?? 0;
      const status = c.status ?? c.Status ?? "Ativa";
      const categoriaPaiId = c.categoriaPaiId ?? c.CategoriaPaiId ?? null;
      return {
        id,
        nomeCategoria: nome,
        slugCategoria: slugify(nome || ""),
        totalProdutos: total,
        status,
        categoriaPaiId,
      };
    });
  } catch (error) {
    console.error("Erro em categoriaService.getAll:", error);
    return [];
  }
};

// Pega categoria por ID
const getById = async (id) => {
  try {
    const response = await api.get(`/categoria/${id}`);
    if (response.status !== 200) throw new Error("Categoria não encontrada");

    const data = response.data || {};
    const cid = data.id ?? data.Id;
    const nome = data.nome ?? data.Nome;
    const total = data.totalProdutos ?? data.TotalProdutos ?? 0;
    const status = data.status ?? data.Status ?? "Ativa";
    const categoriaPaiId = data.categoriaPaiId ?? data.CategoriaPaiId ?? null;
    return {
      id: cid,
      nomeCategoria: nome,
      slugCategoria: slugify(nome || ""),
      totalProdutos: total,
      status,
      categoriaPaiId,
    };
  } catch (error) {
    console.error("Erro em categoriaService.getById:", error);
    throw error;
  }
};

// Criar categoria
const create = async (dto) => {
  try {
    const formData = new FormData();
    formData.append("nome", dto.nome);
    // Se a chave existir no DTO, envie mesmo que seja string vazia para limpar o pai
    if (Object.prototype.hasOwnProperty.call(dto, "categoriaPaiId")) {
      formData.append("categoriaPaiId", dto.categoriaPaiId ?? "");
    }

    const response = await api.post("/categoria", formData);
    return response.data;
  } catch (error) {
    console.error("Erro em categoriaService.create:", error?.response?.data || error);
    throw error;
  }
};

// Atualizar categoria
const update = async (id, dto) => {
  try {
    const formData = new FormData();
    if (dto.nome) formData.append("nome", dto.nome);
    // Se a chave existir no DTO, envie mesmo que seja string vazia para limpar o pai
    if (Object.prototype.hasOwnProperty.call(dto, "categoriaPaiId")) {
      formData.append("categoriaPaiId", dto.categoriaPaiId ?? "");
    }

    await api.patch(`/categoria/${id}`, formData);
    return true;
  } catch (error) {
    console.error("Erro em categoriaService.update:", error?.response?.data || error);
    throw error;
  }
};

// Deletar categoria
const remove = async (id) => {
  try {
    await api.delete(`/categoria/${id}`);
    return true;
  } catch (error) {
    console.error("Erro em categoriaService.remove:", error?.response?.data || error);
    throw error;
  }
};

// Função auxiliar para criar slug
const slugify = (text) => {
  return text
    .toString()
    .toLowerCase()
    .normalize("NFD")
    .replace(/[\u0300-\u036f]/g, "")
    .replace(/\s+/g, "-")
    .replace(/[^\w\-]+/g, "")
    .replace(/\-\-+/g, "-")
    .replace(/^-+/, "")
    .replace(/-+$/, "");
};

export default {
  getAll,
  getById,
  create,
  update,
  remove,
};