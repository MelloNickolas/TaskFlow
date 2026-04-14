import axios from 'axios';


/*
Em vez de usar o axios direto, você cria uma instância personalizada com configurações padrão.
Assim toda requisição que sair pelo HTTPClient já vai com a URL base e dizendo que o conteúdo é JSON
sem precisar repetir isso em cada chamada.
*/
export const HTTPClient = axios.create({
  baseURL: 'https://localhost:7257',
  headers: {
    'Content-Type': 'application/json;charset=UTF-8',
  },
});



/* 
Roda antes de cada requisição sair.
Pega o token salvo no navegador e injeta no header automaticamente. 
Sem isso, todas as rotas protegidas da sua API retornariam 401.
*/
HTTPClient.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

/* 
Roda quando a resposta volta da API. Tem dois caminhos:

Sucesso — só retorna a resposta normal
Erro — se for 401 (token expirado ou inválido), limpa o token do navegador e joga o usuário para a tela de login.
Qualquer outro erro é repassado para quem fez a requisição tratar.
*/
HTTPClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('token');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);