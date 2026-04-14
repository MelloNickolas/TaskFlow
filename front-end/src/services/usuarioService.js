import { HTTPClient } from '../api/Client';

export const usuarioService = {
  listar: async (ativo = true) => {
    try {
      const response = await HTTPClient.get(`/api/usuario/ListarUsuarios?ativo=${ativo}`);
      return response.data;
    } catch (error) {
      console.error('Erro ao listar usuários:', error);
      throw error;
    }
  },

  obterPorId: async (id) => {
    try {
      const response = await HTTPClient.get(`/api/usuario/ObterUsuarioPorId/${id}`);
      return response.data;
    } catch (error) {
      console.error('Erro ao obter usuário:', error);
      throw error;
    }
  },

  criar: async (dados) => {
    try {
      const response = await HTTPClient.post('/api/usuario/CriarUsuario', dados);
      return response.data;
    } catch (error) {
      console.error('Erro ao criar usuário:', error);
      throw error;
    }
  },

  atualizar: async (dados) => {
    try {
      const response = await HTTPClient.put('/api/usuario/AtualizarUsuario', dados);
      return response.data;
    } catch (error) {
      console.error('Erro ao atualizar usuário:', error);
      throw error;
    }
  },

  deletar: async (id) => {
    try {
      const response = await HTTPClient.delete(`/api/usuario/DeletarUsuario/${id}`);
      return response.data;
    } catch (error) {
      console.error('Erro ao deletar usuário:', error);
      throw error;
    }
  },

  restaurar: async (id) => {
    try {
      const response = await HTTPClient.put(`/api/usuario/RestaurarUsuario/${id}`);
      return response.data;
    } catch (error) {
      console.error('Erro ao restaurar usuário:', error);
      throw error;
    }
  },

  atualizarSenha: async (dados) => {
    try {
      const response = await HTTPClient.put('/api/usuario/AtualizarSenhaUsuario', dados);
      return response.data;
    } catch (error) {
      console.error('Erro ao atualizar senha:', error);
      throw error;
    }
  },
};