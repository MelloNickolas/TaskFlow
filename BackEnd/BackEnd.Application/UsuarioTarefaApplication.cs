using BackEnd.Dominio.Entidades;

namespace BackEnd.Application;


public class UsuarioTarefaApplication : IUsuarioTarefaApplication
{
  private readonly IUsuarioTarefaRepository _usuarioTarefaRepository;

  /*
  Estamos injetando dependencias na nossa aplication para podermos usar os métodos dos respectivos repositories para verificarmos
  se os IDs chegam inteiros e não nulos para não quebrar nossa aplicação, como se tivesse assistentes.
  */
  private readonly IUsuarioRepository _usuarioRepository;
  private readonly ITarefaRepository _tarefaRepository;

  public UsuarioTarefaApplication(
      IUsuarioTarefaRepository usuarioTarefaRepository,
      IUsuarioRepository usuarioRepository,
      ITarefaRepository tarefaRepository)
  {
    _usuarioTarefaRepository = usuarioTarefaRepository;
    _usuarioRepository = usuarioRepository;
    _tarefaRepository = tarefaRepository;
  }

  public async Task<UsuarioTarefa?> ObterPorIdAsync(int id)
  {
    var relacionamentoId = await _usuarioTarefaRepository.ObterPorIdAsync(id);
    if (relacionamentoId == null)
      throw new Exception("Relacionamento não encontrado!");

    return relacionamentoId;
  }

  public async Task DeletarAsync(int id)
  {
    var relacionamentoParaDeletar = await ObterPorIdAsync(id);
    await _usuarioTarefaRepository.DeletarAsync(relacionamentoParaDeletar.Id);

  }

  public async Task<IEnumerable<UsuarioTarefa>?> ObterPorTarefaAsync(int tarefaId)
  {
    var verificaTarefaID = await _tarefaRepository.ObterPorIdAsync(tarefaId);
    if (verificaTarefaID == null)
      throw new Exception("Não existe nenhuma TAREFA com esse ID");
    return await _usuarioTarefaRepository.ObterPorTarefaAsync(verificaTarefaID.Id);
  }

  public async Task<IEnumerable<UsuarioTarefa>?> ObterPorUsuarioAsync(int usuarioId)
  {
    var verificaUsuarioID = await _usuarioRepository.ObterPorIdAsync(usuarioId);
    if (verificaUsuarioID == null)
      throw new Exception("Não existe nenhum USUÁRIO com esse ID");
    return await _usuarioTarefaRepository.ObterPorUsuarioAsync(verificaUsuarioID.Id);
  }

  public async Task<int> SalvarAsync(UsuarioTarefa usuarioTarefa)
  {
    var verificaUsuarioID = await _usuarioRepository.ObterPorIdAsync(usuarioTarefa.UsuarioId);
    if (verificaUsuarioID == null)
      throw new Exception("Não existe nenhum USUÁRIO com esse ID");

    var verificaTarefaID = await _tarefaRepository.ObterPorIdAsync(usuarioTarefa.TarefaId);
    if (verificaTarefaID == null)
      throw new Exception("Não existe nenhuma TAREFA com esse ID");


    await _usuarioTarefaRepository.SalvarAsync(usuarioTarefa);
    return usuarioTarefa.Id;
  }
}