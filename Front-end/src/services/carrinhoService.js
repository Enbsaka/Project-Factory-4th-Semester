import api from "./api";
const RESOURCE = "/Pedido"; // usa baseURL central do axios

export const carrinhoService = {
  async getCarrinho(clienteId) {
    try {
      const { data } = await api.get(`${RESOURCE}/carrinho/${clienteId}`);
      return data;
    } catch (err) {
      console.error("Falha ao buscar carrinho:", err);
      throw err;
    }
  },

  async getPedido(pedidoId) {
    try {
      const { data } = await api.get(`/pedido/${pedidoId}`);
      return data;
    } catch (err) {
      console.error("Falha ao buscar pedido:", err);
      throw err;
    }
  },

  async atualizarCarrinho(pedidoId, produtos, clientecpf, freteValor) {
    const body = { produtos, clientecpf };
    if (typeof freteValor !== 'undefined' && freteValor !== null) {
      body.freteValor = Number(freteValor);
    }
    await api.put(`${RESOURCE}/${pedidoId}`, body);
  },

  async aplicarCupom(pedidoId, cupomCodigo) {
    const codigo = String(cupomCodigo || '').trim().toUpperCase();
    if (!codigo) throw new Error('Informe um código de cupom válido');

    // Pré-validação: verifica existência e expiração no servidor
    try {
      const { data } = await api.get(`/Cupom/${codigo}`);
      const ativo = data?.ativo ?? data?.Ativo ?? false;
      const exp = data?.dataExpiracao ?? data?.DataExpiracao ?? null;
      if (!ativo) throw new Error('Cupom inativo.');
      if (exp) {
        const expDt = new Date(exp);
        const agora = new Date();
        if (expDt.getTime() < agora.getTime()) throw new Error('Cupom expirado.');
      }
    } catch (e) {
      // Se 404 ou outra falha, repassa erro amigável
      const msg = e?.response?.data || e?.message || 'Cupom inválido ou expirado.';
      throw new Error(String(msg));
    }

    // Envia como JSON para evitar 415 (Unsupported Media Type)
    await api.patch(`${RESOURCE}/${pedidoId}/cupom`, { cupomCodigo: codigo });
  },

  async finalizarPedido(pedidoId) {
    await api.post(`${RESOURCE}/finalizar/${pedidoId}`);
  }
  ,
  async limparCarrinho(pedidoId, clientecpf) {
    // Limpa o carrinho enviando lista de produtos vazia
    await api.put(`${RESOURCE}/${pedidoId}`, { produtos: [], clientecpf });
  }
};
