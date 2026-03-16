using BackEnd.Dominio.Enumeradores;

namespace BackEnd.Dominio.Entidades;

public class Tarefa
{

  #region Propriedades

  // Id — private set pois é gerado automaticamente pelo banco na criação do registro. Nenhuma camada deve alterá-lo manualmente.
  public int Id { get; private set; }

  /* Titulo  — private set para garantir encpsulamento. Só pode ser alterado pelo método DefinirTitulo,
  que validam se o valor é nulo ou vazio antes de atribuir. */
  public string Titulo { get; private set; }


  /* Descricao  — private set para garantir encpsulamento. */
  public string? Descricao { get; private set; }


  /* DataCriada - Ele encapsula mostrando que essa data nunca vai poder ser alterada pois é instaciada quando
  esse objeto é criado, ou seja, não permitindo alterar ele depois*/
  public DateOnly DataCriada { get; private set; }


  /* Prazo - Ele encapsula mostrando que esse prazo nao pode ser mudado por qualquer um além de garantir que esse
  valor nao pode ser nulo!*/
  public DateOnly Prazo { get; private set; }


  /* Prioridade - Ele encapsula mostrando que esse prioridade nao pode ser mudado por qualquer um além de garantir que esse
  valor nao pode ser nulo e que seja existente dentro dos enumeradores PrioridadeTarefa*/
  public PrioridadeTarefa Prioridade { get; private set; }


  /* Status - Ele garante que o status nao pode ser nulo alem de verificar se ele está dentroi dos enumeradores definidos*/
  public StatusTarefa Status { get; private set; }


  /* UsuariosTarefa — public set e nullable pois o EF Core precisa gerenciar essa coleção internamente para
  carregar os relacionamentos. É nullable pois uma Tarefa pode existir sem ter nenhum usuario atribuído,
  além de ser um ICollection para ser mais flexivel que um list sem se importar com o que acontece por tras de tudo. */
  public ICollection<UsuarioTarefa>? UsuariosTarefa { get; set; }
  // Campo para relacionamento

  #endregion

  public Tarefa()
  {
    DataCriada = DateOnly.FromDateTime(DateTime.Today);
    Status = StatusTarefa.AFazer;
  }

  public void DefinirTitulo(string titulo)
  {
    if (string.IsNullOrWhiteSpace(titulo))
    {
      throw new ArgumentException("O Titulo nao pode ser nulo");
    }
    Titulo = titulo;
  }

  public void DefinirDescricao(string descricao)
  {
    Descricao = descricao;
  }

  public void DefinirPrazo(DateOnly prazo)
  {
    if (prazo < DateOnly.FromDateTime(DateTime.Today))
    {
      throw new ArgumentException("O prazo não pode ser definido no passado");
    }
    Prazo = prazo;
  }

  public void DefinirPrioridade(PrioridadeTarefa prioridade)
  {
    if (!Enum.IsDefined(prioridade))
    {
      throw new ArgumentException("Essa prioridade não existe!");
    }
    Prioridade = prioridade;
  }

  public void DefinirStatus(StatusTarefa status)
  {
    if (!Enum.IsDefined(status))
    {
      throw new ArgumentException("Essa STATUS não existe!");
    }
    Status = status;
  }
}