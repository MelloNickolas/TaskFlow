namespace BackEnd.Dominio.Entidades;

using BackEnd.Dominio.Enumeradores;
public class Usuario
{
  public int Id { get; private set; }
  public string Nome { get; private set; }
  public string Email { get; private set; }
  public UsuarioFuncao Funcao { get; private set; } // ENUM

  // Porque Hash? isso é recomendado para senha, nós vamos gerar um Hash dela e compara--la com a do banco que está cadastrado
  // garantindo assim uma maior segurança
  public string SenhaHash { get; private set; }
  public bool Ativo { get; private set; }

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
