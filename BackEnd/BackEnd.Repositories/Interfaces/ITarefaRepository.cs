using BackEnd.Dominio.Entidades;
using BackEnd.Dominio.Enumeradores;

public interface ITarefaRepository
{
  Task<int> SalvarAsync(Tarefa tarefa);
  Task AtualizarAsync(Tarefa tarefa);
  Task<Tarefa?> ObterPorIdAsync(int id);
  Task<IEnumerable<Tarefa>> ListarPorStatusAsync(StatusTarefa status);
  Task<IEnumerable<Tarefa>> ListarPorPrioridadeAsync(PrioridadeTarefa prioridade);
  Task<IEnumerable<Tarefa>> ListarPorUsuarioAsync(int usuarioId);
  Task DeletarAsync(int id);
}