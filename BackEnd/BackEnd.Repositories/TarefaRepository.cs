using BackEnd.Dominio.Entidades;
using BackEnd.Dominio.Enumeradores;
using Microsoft.EntityFrameworkCore;

public class TarefaRepository : BaseRepository, ITarefaRepository
{
  public TarefaRepository(BackEndContext context) : base(context) { }

  public async Task AtualizarAsync(Tarefa tarefa)
  {
    _context.Tarefas.Update(tarefa);
    await _context.SaveChangesAsync();
  }

  public async Task DeletarAsync(int id)
  {
    var tarefaDeletar = await _context.Tarefas.Where(tarefa => tarefa.Id == id).FirstOrDefaultAsync();
    if (tarefaDeletar != null)
    {
      _context.Tarefas.Remove(tarefaDeletar);
      await _context.SaveChangesAsync();
    }
  }


  public async Task<IEnumerable<Tarefa>?> ListarAsync()
  {
    return await _context.Tarefas.ToListAsync();
  }

  public async Task<IEnumerable<Tarefa>> ListarPorPrioridadeAsync(PrioridadeTarefa prioridade)
  {
    return await _context.Tarefas
    .Where(tarefa => tarefa.Prioridade == prioridade)
    .ToListAsync();
  }

  public async Task<IEnumerable<Tarefa>> ListarPorStatusAsync(StatusTarefa status)
  {
    return await _context.Tarefas
    .Where(tarefa => tarefa.Status == status)
    .ToListAsync();
  }


  /* Aqui como não temos uma propriedade de navegaçõo direta para usuario em tarefa vamos usar o ANY */
  public async Task<IEnumerable<Tarefa>> ListarPorUsuarioAsync(int usuarioId)
  {
    return await _context.Tarefas
  .Where(tarefa => tarefa.UsuariosTarefa != null && tarefa.UsuariosTarefa.Any(ut => ut.UsuarioId == usuarioId))
  .ToListAsync();
  }

  public async Task<Tarefa?> ObterPorIdAsync(int id)
  {
    return await _context.Tarefas
      .Where(tarefa => tarefa.Id == id)
      .FirstOrDefaultAsync();
  }

  public async Task<int> SalvarAsync(Tarefa tarefa)
  {
    await _context.Tarefas.AddAsync(tarefa);
    await _context.SaveChangesAsync();

    return tarefa.Id;
  }
}