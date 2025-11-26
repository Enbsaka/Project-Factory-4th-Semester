// src/services/cupomService.js
import api from "./api";
const RESOURCE = "/Cupom";

export const cupomService = {
  async getAll() {
    const { data } = await api.get(RESOURCE);
    return data;
  },

  async getByCodigo(codigo) {
    const { data } = await api.get(`${RESOURCE}/${codigo}`);
    return data;
  },

  async criarCupom(cupom) {
    const form = new FormData();
    form.append("Codigo", cupom.codigo);
    form.append("DescontoPercentual", cupom.descontoPercentual);
    form.append("DataExpiracao", cupom.dataExpiracao);
    form.append("Ativo", cupom.ativo ? "true" : "false");
    const { data } = await api.post(RESOURCE, form);
    return data;
  },

  async atualizarCupom(id, cupom) {
    const form = new FormData();
    if (cupom.codigo != null) form.append("Codigo", cupom.codigo);
    if (cupom.descontoPercentual != null) form.append("DescontoPercentual", cupom.descontoPercentual);
    if (cupom.dataExpiracao != null) form.append("DataExpiracao", cupom.dataExpiracao);
    if (cupom.ativo != null) form.append("Ativo", cupom.ativo ? "true" : "false");
    await api.patch(`${RESOURCE}/${id}`, form);
  },

  async deletarCupom(id) {
    await api.delete(`${RESOURCE}/${id}`);
  },
};
