using BackEnd.Dominio.Entidades;
using BackEnd.Dominio.Enumeradores;

namespace BackEnd.Application;

public interface ITarefaApplication
{

  // READ
  Task<Tarefa> ObterPorIdAsync(int tarefaId);
  Task<IEnumerable<Tarefa>?> ListarAsync();
  Task<IEnumerable<Tarefa>> ListarPorStatusAsync(StatusTarefa status);
  Task<IEnumerable<Tarefa>> ListarPorPrioridadeAsync(PrioridadeTarefa prioridade);
  Task<IEnumerable<Tarefa>> ListarPorUsuarioAsync(int usuarioId);

  // CREATE
  Task<int> CriarAsync(Tarefa tarefaDTO);

  // UPDATE
  Task AtualizarAsync(Tarefa tarefaDTO);

  // DELETE
  Task DeletarAsync(int id);
}