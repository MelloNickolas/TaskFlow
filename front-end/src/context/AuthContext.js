import { createContext, useContext, useState, useEffect } from 'react';
import { authService } from '../services/authService';


/*
primeiro criamos o AuthContext com o createContext para criar um objeto de contexto que permite compartilhar dados globalmente
(como temas, usuário autenticado) entre componentes sem "prop drilling" (passar props manualmente por todos os níveis).
*/
const AuthContext = createContext(null);


export function AuthProvider({ children }) {
  /* 
  O useState(null) aqui é só um estado que guarda os dados do usuário logado — nome, email, função, etc. 
  Começa como null porque quando a aplicação abre, ainda não tem ninguém logado.
  
  usuario — o valor atual, você lê ele
  setUsuario — a função para atualizar ele
  
  Então o fluxo seria:
  Aplicação abre → usuario é null (ninguém logado)
  Usuário faz login → setUsuario({ nome: 'Ana', funcao: 3 })
  Agora usuario tem os dados e qualquer componente pode acessar
  */
  const [usuario, setUsuario] = useState(null);


  /* 
  Quando a página recarrega, o useEffect roda logo na montagem
  e verifica se já existe um token no localStorage.
  Se existir, seta o usuario para não perder a sessão.
  */
  const [carregando, setCarregando] = useState(true);
  useEffect(() => {
    const token = localStorage.getItem('token');
    if (token) {
      setUsuario({ token });
    }
    setCarregando(false);
  }, []);

  const login = async (email, senha) => {
    const data = await authService.login(email, senha);
    authService.salvarToken(data.token);
    setUsuario(data);
  };

  const logout = () => {
    authService.logout();
    setUsuario(null);
  };


  return (

    /* 
    AuthProvider é um componente que vai abraçar toda sua aplicação. Pensa assim:
    <AuthProvider>
      <App />
    </AuthProvider>
    Tudo que estiver dentro dele tem acesso aos dados do contexto.
    
    children é justamente esse conteúdo interno — no caso acima seria o <App />.
    É como se o AuthProvider dissesse: "recebo qualquer coisa dentro de mim e exibo ela".
    
    value={{ usuario }} é o que você está disponibilizando para todos os componentes filhos. 
    */
    <AuthContext.Provider value={{ usuario, login, logout, carregando }}>
      {children}
    </AuthContext.Provider>
  );
}

/* é um hook personalizado que encapsula o useContext(AuthContext) para facilitar o uso em qualquer componente.  */
export function useAuth() {
  return useContext(AuthContext);
}