using BackEnd.Dominio.Entidades;

namespace BackEnd.Services.Interfaces;

public interface IJwtService
{
  /* 
  Estamos criando um método que vai gerar um TOKEN e será armazenado em uma string,
  esse token vai vir de um Objeto Usuario, ele que vai fazer o login.
  */
  string GerarToken(Usuario usuario);


  /* 
  VÁ PARA O APPSETTINGS.JSON DEFINIR SUA CHAVE!, como nao podemos comentar lá, entenda aqui o cada um faz:
  Issuer — quem emitiu o token, geralmente a URL da sua API
  Audience — para quem o token é destinado, geralmente a URL do frontend
  Key — chave secreta para assinar o token, quanto maior e mais aleatória melhor
  */
}