namespace BackEnd.Dominio.Entidades;

using BackEnd.Dominio.Enumeradores;
public class Usuario
{
  #region Propriedades

  // Id — private set pois é gerado automaticamente pelo banco na criação do registro. Nenhuma camada deve alterá-lo manualmente.
  public int Id { get; private set; }


  /* Nome  — private set para garantir encpsulamento. Só pode ser alterado pelo método DefinirNome,
  que validam se o valor é nulo ou vazio antes de atribuir. */
  public string Nome { get; private set; }


  /*Email — private set para garantir encapsulamento. Só pode ser alterado pelo método DefinirEmail,
  que valida se o valor é nulo ou vazio antes de atribuir.*/
  public string Email { get; private set; }


  /* SenhaHash — private set por três motivos: a senha nunca é armazenada em texto puro por segurança,\
  o hash é gerado na camada de Aplicação e apenas armazenado aqui, e o encapsulamento garante que
   só pode ser alterado pelo método DefinirSenhaHash que valida se não é nulo.*/
  public string SenhaHash { get; private set; }


  /* FuncaoUsuario — private set para garantir encapsulamento. Só pode ser alterado pelo método DefinirFuncao
  que valida se o valor existe nos enumeradores de UsuarioFuncao, evitando que uma função inválida seja atribuída.*/
  public UsuarioFuncao Funcao { get; private set; } // ENUM


  /* Ativo — private set pois é inicializado como true no construtor e só pode ser alterado pelos métodos Deletar()
  e Recuperar(), que representam o soft delete do sistema.*/
  public bool Ativo { get; private set; }

  
  /* UsuarioTarefas — public set e nullable pois o EF Core precisa gerenciar essa coleção internamente para
  carregar os relacionamentos. É nullable pois um usuário pode existir sem ter nenhuma tarefa atribuída, além de ser um ICollection para ser mais flexivel que um list sem se importar com o que acontece por tras de tudo. */
  public ICollection<UsuarioTarefa>? UsuarioTarefas { get; set; }
  // Campo para o relacionamento!


  #endregion


  public Usuario()
  {
    Ativo = true;
  }

  public void Deletar()
  {
    Ativo = false;
  }
  public void Recuperar()
  {
    Ativo = true;
  }

  /// <summary>
  /// Define o hash da senha do usuário.
  /// A senha nunca é armazenada em texto puro por motivos de segurança.
  /// Antes de chamar este método, a senha informada pelo usuário deve ser
  /// transformada em um hash (ex: BCrypt) na camada de serviço/autenticação.
  /// Este método apenas recebe o hash gerado e o armazena na entidade.
  /// </summary>
  public void DefinirSenhaHash(string hash)
  {
    if (string.IsNullOrWhiteSpace(hash))
    {
      throw new ArgumentException("A senha não pode ser nula!");
    }
    SenhaHash = hash;
  }

  /// <summary>
  /// Vamos criar os métodos para realizar os private setters dos nomes
  /// </summary>

  public void DefinirNome(string nome)
  {
    if (string.IsNullOrWhiteSpace(nome))
    {
      throw new ArgumentException("O nome não pode ser nulo!");
    }
    Nome = nome;
  }
  public void DefinirEmail(string email)
  {
    if (string.IsNullOrWhiteSpace(email))
    {
      throw new ArgumentException("O email não pode ser nulo!");
    }
    Email = email;
  }
  public void DefinirFuncao(UsuarioFuncao funcao)
  {
    if (!Enum.IsDefined(funcao))
    {
      throw new ArgumentException("A FUNÇÃO não existe!");
    }
    Funcao = funcao;
  }
}
