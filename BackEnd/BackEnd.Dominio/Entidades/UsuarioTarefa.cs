namespace BackEnd.Dominio.Entidades;

public class UsuarioTarefa
{
  public int Id { get; private set; }
  public DateOnly DataAtribuicao { get; private set; }

  public int UsuarioId { get; private set; }
  public Usuario Usuario { get; set; }

  public int TarefaId { get; private set; }
  public Tarefa Tarefa { get; set; }

  public UsuarioTarefa(int usuarioId, int tarefaId)
  {
    DataAtribuicao = DateOnly.FromDateTime(DateTime.Today);
    UsuarioId = usuarioId;
    TarefaId = tarefaId;
  }
}

// UsuarioTarefa representa o relacionamento N:N (muitos para muitos) entre Usuario e Tarefa.
// Uma tarefa pode ter vários responsáveis, e um usuário pode participar de várias tarefas.
//
// Utilizamos FKs (UsuarioId e TarefaId) para referenciar as entidades  relacionadas,
// junto com as propriedades de navegação (Usuario e Tarefa) para o Entity Framework.
//
// O vínculo é imutável — UsuarioId e TarefaId são definidos apenas na criação
// pelo construtor e não podem ser alterados. Caso seja necessário reatribuir,
// o registro é deletado e um novo é criado.
//
// DataAtribuicao é setada automaticamente no construtor com a data atual.