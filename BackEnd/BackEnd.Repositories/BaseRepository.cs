/*
abstract — significa que você não pode criar uma instância direta do BaseRepository.
Ela existe para ser herdada por outros repositórios.
Por exemplo, UsuarioRepository vai herdar dela e ter acesso ao _context automaticamente.

protected — significa que o _context só é acessível pela própria classe e pelas classes que herdam dela. 
Nenhuma classe de fora consegue acessar diretamente.

Faz sentido? A ideia é que todos os repositórios do projeto herdem do BaseRepository e já ganhem o _context pronto para usar.
*/

public abstract class BaseRepository
{
  protected readonly BackEndContext _context;

  protected BaseRepository(BackEndContext context)
  {
    _context = context;
  }
}