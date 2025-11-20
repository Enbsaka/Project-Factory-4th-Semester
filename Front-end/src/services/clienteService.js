import api from "./api";
const RESOURCE = "/Cliente";

export default {
  // Obter perfil do cliente logado
  async getMeuPerfil() {
    try {
      // Evita 401 no admin: não há perfil de cliente para tokens de Admin
      const role = (localStorage.getItem('role') || '').toLowerCase();
      if (role === 'admin') {
        return null;
      }
      const response = await api.get(`${RESOURCE}/me`);
      return response.data;
    } catch (error) {
      console.error('Erro ao obter perfil do cliente:', error);
      return null;
    }
  },
  // Buscar todos os clientes (com suporte à paginação)
  async getAllClientes(pagina = 1, tamanhoPagina = 10) {
    try {
      const response = await api.get(`${RESOURCE}`, { params: { pagina, tamanhoPagina } });
      const data = response.data;

      // retorna um objeto mais organizado
      return {
        clientes: data.itens || [],
        totalItens: data.totalItens || 0,
        paginaAtual: data.paginaAtual || pagina,
        totalPaginas: data.totalPaginas || 1
      };
    } catch (error) {
      console.error("Erro ao buscar clientes:", error);
      return { clientes: [], totalItens: 0, paginaAtual: 1, totalPaginas: 1 };
    }
  },

  // Buscar cliente por ID
  async getClienteById(id) {
    try {
      const response = await api.get(`${RESOURCE}/${id}`);
      return response.data;
    } catch (error) {
      console.error(`Erro ao buscar cliente ${id}:`, error);
      return null;
    }
  },

  // Criar novo cliente
  async createCliente(clienteData) {
    try {
      // Endpoint espera [FromForm]: enviar como x-www-form-urlencoded
      const params = new URLSearchParams();
      Object.keys(clienteData).forEach((key) => {
        const value = clienteData[key] ?? "";
        params.append(key, value);
      });
      const response = await api.post(RESOURCE, params, {
        headers: { "Content-Type": "application/x-www-form-urlencoded" },
      });
      return response.data;
    } catch (error) {
      console.error("Erro ao criar cliente:", error);
      // Propaga mensagem do backend para o chamador
      throw new Error(error?.response?.data || "Falha ao criar cliente");
    }
  },

  // Atualizar cliente existente
  async updateCliente(id, clienteData) {
    try {
      // Endpoint PATCH espera JSON (sem [FromForm])
      await api.patch(`${RESOURCE}/${id}`, clienteData);
      return true;
    } catch (error) {
      console.error(`Erro ao atualizar cliente ${id}:`, error);
      return false;
    }
  },

  // Deletar cliente
  async deleteCliente(id) {
    try {
      await api.delete(`${RESOURCE}/${id}`);
      return true;
    } catch (error) {
      console.error(`Erro ao deletar cliente ${id}:`, error);
      return false;
    }
  }
};
