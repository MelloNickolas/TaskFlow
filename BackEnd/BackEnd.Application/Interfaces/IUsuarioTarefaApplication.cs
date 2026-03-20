using BackEnd.Dominio.Entidades;

public interface IUsuarioTarefaApplication
{
  public Task<int> SalvarAsync(UsuarioTarefa usuarioTarefa);
  Task<UsuarioTarefa?> ObterPorIdAsync(int id);
  Task<IEnumerable<UsuarioTarefa>?> ObterPorUsuarioAsync(int usuarioId);
  Task<IEnumerable<UsuarioTarefa>?> ObterPorTarefaAsync(int tarefaId);
  Task DeletarAsync(int id);

}