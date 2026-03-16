  using BackEnd.Dominio.Entidades;


  /*
  Vamos usar a interface para ter controle sobre os métodos dos repositories.
  Além disso, a camada de Aplicação vai depender da interface e não da implementação concreta — isso se chama inversão de dependência.
  */
  public interface IUsuarioRepository
  {
    /*Task indica que o método é assíncrono — ele não bloqueia o fluxo da aplicação enquanto espera uma operação demorada,
    como uma consulta ao banco de dados. O chamador usa await para aguardar o resultado sem travar a thread.*/


    /* Quando salvar ele retornará Int, seria o Id do usuário, e como parâmetro passamos um objeto Usuario*/
    Task<int> SalvarAsync(Usuario usuario);


    /* Quando Atualizar ele retornará nada, eu mostrarei uma mensagem, poderia ser void mas optei por nao ter nada*/
    Task AtualizarAsync(Usuario usuario);


    /* Retornará um objeto do tipo Usuario, e passará um id do usuario como parâmetro para busca, pode nao achar nada */
    Task<Usuario?> ObterPorIdAsync(int id);


    /* Retornará um objeto do tipo Usuario, e passará um email do usuario como parâmetro para busca, pode nao achar nada */
    Task<Usuario?> ObterPorEmailAsync(string email);


    /* Retornará um objeto do tipo lista enumerada, e passará um status para filtrar se quer ver os ativos ou inativos */
    Task<IEnumerable<Usuario>> ListarAsync(bool ativo);

    // Nao temos um método deletar pois no dominio temos um soft delete, um método que ja faz isso
  }