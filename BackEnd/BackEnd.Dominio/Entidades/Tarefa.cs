using BackEnd.Dominio.Enumeradores;

namespace BackEnd.Dominio.Entidades;

public class Tarefa
{
  public int Id {get; private set;}
  public string Titulo {get; private set;}
  public string? Descricao{ get; private set;}
  public DateOnly DataCriada {get; private set;}
  public DateOnly Prazo {get; private set;}
  public PrioridadeTarefa Prioridade {get; private set;}
  public StatusTarefa Status {get; private set;}

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