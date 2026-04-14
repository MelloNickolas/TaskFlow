import React from 'react';
import ReactDOM from 'react-dom/client';
import './index.css';
import App from './App';
import reportWebVitals from './reportWebVitals';
import { AuthProvider } from './context/AuthContext';

/* 
 O AuthProvider não bloqueia o acesso — ele só disponibiliza o contexto de autenticação para toda a aplicação.
Pensa assim: ele é como uma mochila que carrega as informações do usuário logado. 
Todo componente dentro do App pode abrir essa mochila e pegar o usuario, login ou logout quando precisar.
O bloqueio de acesso às páginas vai ser feito por uma rota protegida — que a gente vai criar mais pra frente. 
Ela vai verificar se tem usuário logado, e se não tiver redireciona para o login.
*/

const root = ReactDOM.createRoot(document.getElementById('root'));
root.render(
  <React.StrictMode>
    <AuthProvider>
      <App />
    </AuthProvider>
  </React.StrictMode>
);

reportWebVitals();
