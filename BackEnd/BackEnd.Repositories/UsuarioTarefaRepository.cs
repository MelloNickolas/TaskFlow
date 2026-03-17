
using BackEnd.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories;
public class UsuarioTarefaRepository : BaseRepository, IUsuarioTarefaRepository
{
  public UsuarioTarefaRepository(BackEndContext context) : base(context){}

  /* Vamos fazer o delete pelo o ID do relacionamento entre os dois */
  public async Task DeletarAsync(int id)
  {
    var usuarioTarefaDeletar = await _context.UsuariosTarefas.Where(ut => ut.Id == id).FirstOrDefaultAsync();
    if(usuarioTarefaDeletar != null)
    {
      _context.Remove(usuarioTarefaDeletar);
      await _context.SaveChangesAsync();
    }
  }

  public async Task<IEnumerable<UsuarioTarefa>?> ObterPorTarefaAsync(int tarefaId)
  {
    return await _context.UsuariosTarefas.Where(usuarioTarefa => usuarioTarefa.TarefaId == tarefaId).ToListAsync();
  }

  public async Task<IEnumerable<UsuarioTarefa>?> ObterPorUsuarioAsync(int usuarioId)
  {
    return await _context.UsuariosTarefas.Where(usuarioTarefa => usuarioTarefa.UsuarioId == usuarioId).ToListAsync();
  }

  public async Task<int> SalvarAsync(UsuarioTarefa usuarioTarefa)
  {
    await _context.UsuariosTarefas.AddAsync(usuarioTarefa);
    await _context.SaveChangesAsync();

    return usuarioTarefa.Id;
  }
}