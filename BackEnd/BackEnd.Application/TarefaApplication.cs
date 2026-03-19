using BackEnd.Dominio.Entidades;
using BackEnd.Dominio.Enumeradores;

namespace BackEnd.Application;

public class TarefaApplication : ITarefaApplication
{
  readonly ITarefaRepository _tarefaRepository;
  public TarefaApplication(ITarefaRepository tarefaRepository) { _tarefaRepository = tarefaRepository; }


  public async Task<Tarefa> ObterPorIdAsync(int tarefaId)
  {
    var tarefaIdExistente = await _tarefaRepository.ObterPorIdAsync(tarefaId);
    if (tarefaIdExistente == null)
    {
      throw new Exception("Não existe nenhuma tarefa com esse ID!");
    }
    return tarefaIdExistente;
  }

  public async Task<IEnumerable<Tarefa>?> ListarAsync()
  {
    return await _tarefaRepository.ListarAsync();
  }



  public async Task<IEnumerable<Tarefa>> ListarPorPrioridadeAsync(PrioridadeTarefa prioridade)
  {
    return await _tarefaRepository.ListarPorPrioridadeAsync(prioridade);
  }

  public async Task<IEnumerable<Tarefa>> ListarPorStatusAsync(StatusTarefa status)
  {
   return await _tarefaRepository.ListarPorStatusAsync(status);
  }

  public async Task<IEnumerable<Tarefa>> ListarPorUsuarioAsync(int usuarioId)
  {
    return await _tarefaRepository.ListarPorUsuarioAsync(usuarioId);
  }

  public async Task AtualizarAsync(Tarefa tarefaDTO)
  {
    await ObterPorIdAsync(tarefaDTO.Id);
    await _tarefaRepository.AtualizarAsync(tarefaDTO);
  }

  public async Task<int> CriarAsync(Tarefa tarefaDTO)
  {
    return await _tarefaRepository.SalvarAsync(tarefaDTO);
  }

  public async Task DeletarAsync(int id)
  {
    var tarefaParaDeletar = await ObterPorIdAsync(id);
    await _tarefaRepository.DeletarAsync(tarefaParaDeletar.Id);
  }



}