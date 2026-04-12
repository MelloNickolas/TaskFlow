using System.Threading.Tasks;
using BackEnd.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories;


/* 
herdamos o repositorio Base para fazermos nossos métodos acessarem o banco através da string de conexão
Além de termos os métodos das interfaces herdados
*/
public class UsuarioRepository : BaseRepository, IUsuarioRepository
{
  /*puxando e armazenando nosso contexto (configs, strings de conexao) de conexao em um construtor*/
  public UsuarioRepository(BackEndContext context) : base(context) { }


  /* adicionamos um async onde add um usuario e salvamos ele no contexto retornando o Id*/
  public async Task<int> SalvarAsync(Usuario usuario)
  {
    await _context.Usuarios.AddAsync(usuario);
    await _context.SaveChangesAsync();
    return usuario.Id;
  }


  /* Somente salvamos um objeto usuario que chegar*/
  public async Task AtualizarAsync(Usuario usuario)
  {
    _context.Usuarios.Update(usuario);
    await _context.SaveChangesAsync();
  }

  /* Só retornamos o Usuario que acharmos igual ao id que será passado de parâmetro */
  public async Task<Usuario?> ObterPorIdAsync(int id)
  {
    return await _context.Usuarios.Where(u => u.Id == id).FirstOrDefaultAsync();
  }


  /* Só retornamos o Usuario que acharmos igual ao email que será passado de parâmetro */
  public async Task<Usuario?> ObterPorEmailAsync(string email)
  {
    return await _context.Usuarios
    .Where(u => u.Email == email)
    .Where(u => u.Ativo == true)
    .FirstOrDefaultAsync();

  }


  /* Vamos retornar uma lista numerada que mostrará todos os usuarios ativos */
  public async Task<IEnumerable<Usuario>> ListarAsync(bool ativo)
  {
    return await _context.Usuarios
    .Include(u => u.UsuarioTarefas)
    .Where(u => u.Ativo == ativo).ToListAsync();
  }
}