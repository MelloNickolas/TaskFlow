using BackEnd.Dominio.Entidades;

namespace BackEnd.Application;

public interface IUsuarioApplication
{
  /*
  Método de login, onde estamos retornando uma string que vai ser o nosso token JWT
  */
  Task<string> LoginAsync(string email, string senha);


  /* 
  Para você entender aqui vou criar os métodos que vão ser aplicadas as regras de validação 
  Exemplo, somente o admin vai poder inativar usuarios, é aqui que vamos validar essa idéia
  */

  // READ
  Task<Usuario> ObterPorIdAsync(int usuarioId);
  Task<Usuario> ObterPorEmailAsync(string email);
  Task<IEnumerable<Usuario>?> ListarAsync(bool ativo);

  // CREATE
  // Aqui vamos pegar a senha de parâmetro e passar ela para um Hash, por isso ela está aqui
  Task<int> CriarAsync(Usuario usuarioDTO, string senha);

  // UPDATE
  Task AtualizarAsync(Usuario usuarioDTO);
  Task AtualizarSenhaAsync(Usuario usuarioDTO, string senhaAntiga, string senhaNova);


  // DELETE e RECUPERAR
  Task DeletarAsync(int usuarioId);
  Task RestaurarAsync(int usuarioId);

}