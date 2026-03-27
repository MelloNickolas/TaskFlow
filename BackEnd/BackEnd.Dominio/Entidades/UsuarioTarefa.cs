namespace BackEnd.Dominio.Entidades;

public class UsuarioTarefa
{

  #region Propriedades

  // Id — private set pois é gerado automaticamente pelo banco na criação do registro. Nenhuma camada deve alterá-lo manualmente.
  public int Id { get; private set; }


  /* DataAtribuicao - Ele encapsula mostrando que essa data nao pode ser mudado pois é iniciado assim que é atribuido!*/
  public DateOnly DataAtribuicao { get; private set; }


  /* UsuarioId  private set- Sempre que ele for criado ele nao pode ser alterado, ou seja ele somente pode ser excluido e nao pode ser nulo
  Usuario public set - Alem de chamarmos a entidade usuario para podermos ter o acesso do mesmo e termos o public set para manejo do ef core*/
  public int UsuarioId { get; private set; }
  public Usuario Usuario { get; set; }


  /* TarefaId  private set- Sempre que ele for criado ele nao pode ser alterado, ou seja ele somente pode ser excluido e nao pode ser nulo
  Tarefa public set - Alem de chamarmos a entidade tarefa para podermos ter o acesso do mesmo e termos o public set para manejo do ef core*/
  public int TarefaId { get; private set; }
  public Tarefa Tarefa { get; set; }
  #endregion


  

  public UsuarioTarefa(int usuarioId, int tarefaId)
  {
    DataAtribuicao = DateOnly.FromDateTime(DateTime.Today);

    if (usuarioId <= 0)
    {
      throw new ArgumentException("A relaçao deve posssuir ao menos um Usuario");
    }
    UsuarioId = usuarioId;


    if (tarefaId <= 0)
    {
      throw new ArgumentException("A relaçao deve posssuir ao menos uma Tarefa");
    }
    TarefaId = tarefaId;


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
  }

  public void DefinirUsuario(int usuarioId)
  {
    UsuarioId = usuarioId;
  }
  
  public void DefinirTarefa(int tarefaId)
  {
    TarefaId = tarefaId;
  }
}

