import { HTTPClient } from '../api/Client';


/* 
depois vamos pegar os parametros email e senha que vao ser requisitados e 
vamos enviar eles para a rota /api/auth/Login, passando eles e retornar a os dados da nossa resposta, 
que seria o nosso token ou erro.


O async/await está aí porque a requisição para a API leva um tempo indeterminado — o JavaScript não sabe quanto tempo o servidor vai demorar para responder.
Sem o await, o código continuaria executando antes da resposta chegar, e o response estaria vazio.
O async/await basicamente diz: "espera a API responder antes de continuar".
A verificação de email e senha em si acontece no backend — o front só envia e aguarda a resposta. 
*/



/* 
Resumindo o fluxo completo:

1 - Usuário digita email e senha na tela de login
2 - login() envia para a API e aguarda a resposta
3 - A API valida e retorna o token
4 - salvarToken() guarda o token no localStorage
*/
export const authService = {
  login: async (email, senha) => {
    const response = await HTTPClient.post('/api/auth/Login', { email, senha });
    return response.data;
  },



  /*
  é que uma variável normal some quando a página é recarregada. 
  O localStorage persiste — ou seja, se o usuário fechar o navegador e abrir de novo, 
  o token ainda está lá e ele continua logado sem precisar fazer login de novo.
  */
  salvarToken: (token) => {
    localStorage.setItem('token', token);
  },

  /* ele so remove o token do localStorage, ou seja nao fica mais armazenado */
  logout: () => {
  localStorage.removeItem('token');
},
};