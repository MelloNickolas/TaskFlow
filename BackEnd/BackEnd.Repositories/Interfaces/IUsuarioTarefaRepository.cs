using BackEnd.Dominio.Entidades;

public interface IUsuarioTarefaRepository
{

  /* O método quando salva retorna o Id do relacionameto */
  Task<int> SalvarAsync(UsuarioTarefa usuarioTarefa);

  // Buscar a tarefa, vamos usar só para o delete
  Task<UsuarioTarefa?> ObterPorIdAsync(int id);


  /* Vai retornar uma list pois quero buscar todas as tarefas que o usuário tem relacionado */
  Task<IEnumerable<UsuarioTarefa>?> ObterPorUsuarioAsync(int usuarioId);


  /* Vai retornar uma list pois quero buscar todas os usuários que a tarefa tem relacionada */
  Task<IEnumerable<UsuarioTarefa>?> ObterPorTarefaAsync(int tarefaId);

  /* Como nao vamos ter update nesse projeto porque nao tem sentido, temos o Delete pelo id do relacionamento */
  Task DeletarAsync(int id);
}