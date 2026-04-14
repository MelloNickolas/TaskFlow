import { HTTPClient } from '../api/Client';

export const atribuicaoService = {
  criarRelacionamento: async (dados) => {
    try {
      const response = await HTTPClient.post('/api/usuariotarefa/CriarRelacionamento', dados);
      return response.data;
    } catch (error) {
      console.error('Erro ao criar atribuição:', error);
      throw error;
    }
  },

  obterPorUsuario: async (usuarioId) => {
    try {
      const response = await HTTPClient.get(`/api/usuariotarefa/ObterRelacionamentoPorUsuario?usuarioId=${usuarioId}`);
      return response.data;
    } catch (error) {
      console.error('Erro ao obter atribuições por usuário:', error);
      throw error;
    }
  },

  obterPorTarefa: async (tarefaId) => {
    try {
      const response = await HTTPClient.get(`/api/usuariotarefa/ObterRelacionamentoPorTarefa?tarefaId=${tarefaId}`);
      return response.data;
    } catch (error) {
      console.error('Erro ao obter atribuições por tarefa:', error);
      throw error;
    }
  },

  deletarRelacionamento: async (id) => {
    try {
      const response = await HTTPClient.delete(`/api/usuariotarefa/DeletarRelacionamento/${id}`);
      return response.data;
    } catch (error) {
      console.error('Erro ao deletar atribuição:', error);
      throw error;
    }
  },
};