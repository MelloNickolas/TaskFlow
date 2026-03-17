using BackEnd.Dominio.Entidades;

namespace BackEnd.Application;

/* Aqui não vamos fazer a verificaçao de valores nullable porque no nosso Dominio nós ja fizemos isso */
public class UsuarioApplication : IUsuarioApplication
{

  /*
  A UsuarioApplication recebe o IUsuarioRepository pelo construtor — isso se chama injeção de dependência. 
  Ela não sabe como o repositório funciona por dentro, só sabe que ele tem os métodos definidos na interface.
  Isso é importante porque se um dia você trocar o banco de dados ou a implementação do repositório, 
  a Application não precisa mudar nada — ela continua usando a mesma interface.
  */
  private readonly IUsuarioRepository _usuarioRepository;
  public UsuarioApplication(IUsuarioRepository usuarioRepository) { _usuarioRepository = usuarioRepository; }




  public async Task<Usuario> ObterPorIdAsync(int usuarioId)
  {
    var usuarioIdExistente = await _usuarioRepository.ObterPorIdAsync(usuarioId);
    if (usuarioIdExistente == null)
      throw new Exception("Usuário não encontrado!");

    return usuarioIdExistente;
  }

    public async Task<Usuario> ObterPorEmailAsync(string email)
  {
    var usuarioEmailExistente = await _usuarioRepository.ObterPorEmailAsync(email);
    if (usuarioEmailExistente == null)
      throw new Exception("Email não encontrado!");

    return usuarioEmailExistente;
  }




  public async Task<int> CriarAsync(Usuario usuarioDTO, string senha)
  {

    /* Aqui vamos validar nossa regra de negocio e conferir se o e-mail que for cadastrar já existe */
    var usuarioEmailExistente = await _usuarioRepository.ObterPorEmailAsync(usuarioDTO.Email);
    if (usuarioEmailExistente != null)
      throw new Exception("Email já cadastrado!");

    /* Aqui vamos pegar a senha que veio como parâmetro e vamos passa-la criptografa-la e salvar no nosso campo SenhaHash */
    var Hash = BCrypt.Net.BCrypt.HashPassword(senha);
    usuarioDTO.DefinirSenhaHash(Hash);

    return await _usuarioRepository.SalvarAsync(usuarioDTO);
  }



  public async Task AtualizarSenhaAsync(Usuario usuarioDTO, string senhaAntiga, string senhaNova)

  /* Verifico se a senhaAntiga e que está guardada coincidem, se não coiincidirem quebra, se não continua */
  {
    if (!BCrypt.Net.BCrypt.Verify(senhaAntiga, usuarioDTO.SenhaHash))
      throw new Exception("As senhas não coincidem!");

    /* Aqui chamamos o método e definimos a nova senha, alem de atualizar ele já */
    usuarioDTO.DefinirSenhaHash(BCrypt.Net.BCrypt.HashPassword(senhaNova));
    await _usuarioRepository.AtualizarAsync(usuarioDTO);
  }



  public async Task AtualizarAsync(Usuario usuarioDTO)
  {
    /* Verifica se o Id de busca é existente, isso com o método que ja fizemos no começo */
    await ObterPorIdAsync(usuarioDTO.Id);
    /* Atualiza */
    await _usuarioRepository.AtualizarAsync(usuarioDTO);
  }



  public async Task DeletarAsync(int usuarioId)
  {
    /* Verifica se o Id de busca é existente */
    var usuarioIdExistente = await ObterPorIdAsync(usuarioId);

    /* Chamamos o método do Dominio e realizamos um soft delete */
    usuarioIdExistente.Deletar();
    await _usuarioRepository.AtualizarAsync(usuarioIdExistente);
  }


  public async Task RestaurarAsync(int usuarioId)
  {
    /* Verifica se o Id de busca é existente */
    var usuarioIdExistente = await ObterPorIdAsync(usuarioId);

    /* Chamamos o método do Dominio e realizamos um soft recupere */
    usuarioIdExistente.Recuperar();
    await _usuarioRepository.AtualizarAsync(usuarioIdExistente);
  }

  public async Task<IEnumerable<Usuario>?> ListarAsync(bool ativo)
  {
    return await _usuarioRepository.ListarAsync(ativo);
  }

}