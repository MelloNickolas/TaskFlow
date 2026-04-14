import { HTTPClient } from '../api/Client';

export const dashboardService = {
  resumoTarefas: async () => {
    try {
      const response = await HTTPClient.get('/api/relatorio/ResumoTarefas');
      return response.data;
    } catch (error) {
      console.error('Erro ao obter resumo de tarefas:', error);
      throw error;
    }
  },

  tarefasRecentes: async () => {
    try {
      const response = await HTTPClient.get('/api/relatorio/TarefasRecentes');
      return response.data;
    } catch (error) {
      console.error('Erro ao obter tarefas recentes:', error);
      throw error;
    }
  },

  progressoGeral: async () => {
    try {
      const response = await HTTPClient.get('/api/relatorio/ProgressoGeral');
      return response.data;
    } catch (error) {
      console.error('Erro ao obter progresso geral:', error);
      throw error;
    }
  },

  resumoEquipe: async () => {
    try {
      const response = await HTTPClient.get('/api/relatorio/ResumoEquipe');
      return response.data;
    } catch (error) {
      console.error('Erro ao obter resumo da equipe:', error);
      throw error;
    }
  },
};