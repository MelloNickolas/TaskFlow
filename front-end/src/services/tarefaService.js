import { HTTPClient } from '../api/Client';

export const tarefaService = {

  listar: async () => {
    try {
      const response = await HTTPClient.get('/api/tarefa/ListarTarefas');
      return response.data;
    } catch (error) {
      console.error('Erro ao listar tarefas:', error);
      throw error;
    }
  },

  listarPorStatus: async (status) => {
    try {
      const response = await HTTPClient.get(`/api/tarefa/ListarTarefasPorStatus?statusTarefa=${status}`);
      return response.data;
    } catch (error) {
      console.error('Erro ao listar tarefas por status:', error);
      throw error;
    }
  },

  listarPorPrioridade: async (prioridade) => {
    try {
      const response = await HTTPClient.get(`/api/tarefa/ListarTarefasPorPrioridade?prioridadeTarefa=${prioridade}`);
      return response.data;
    } catch (error) {
      console.error('Erro ao listar tarefas por prioridade:', error);
      throw error;
    }
  },

  listarPorUsuario: async (usuarioId) => {
    try {
      const response = await HTTPClient.get(`/api/tarefa/ListarTarefasPorUsuario?IdUsuario=${usuarioId}`);
      return response.data;
    } catch (error) {
      console.error('Erro ao listar tarefas por usuário:', error);
      throw error;
    }
  },

  obterPorId: async (id) => {
    try {
      const response = await HTTPClient.get(`/api/tarefa/ObterTarefaPorId/${id}`);
      return response.data;
    } catch (error) {
      console.error('Erro ao obter tarefa:', error);
      throw error;
    }
  },

  criar: async (dados) => {
    try {
      const response = await HTTPClient.post('/api/tarefa/CriarTarefa', dados);
      return response.data;
    } catch (error) {
      console.error('Erro ao criar tarefa:', error);
      throw error;
    }
  },

  atualizar: async (dados) => {
    try {
      const response = await HTTPClient.put('/api/tarefa/AtualizarTarefa', dados);
      return response.data;
    } catch (error) {
      console.error('Erro ao atualizar tarefa:', error);
      throw error;
    }
  },

  atualizarStatus: async (dados) => {
    try {
      const response = await HTTPClient.put('/api/tarefa/AtualizarStatusTarefa', dados);
      return response.data;
    } catch (error) {
      console.error('Erro ao atualizar status da tarefa:', error);
      throw error;
    }
  },

  deletar: async (id) => {
    try {
      const response = await HTTPClient.delete(`/api/tarefa/DeletarTarefa/${id}`);
      return response.data;
    } catch (error) {
      console.error('Erro ao deletar tarefa:', error);
      throw error;
    }
  },
};