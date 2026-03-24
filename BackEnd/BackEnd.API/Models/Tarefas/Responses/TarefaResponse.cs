using BackEnd.Dominio.Enumeradores;

namespace BackEnd.API.Models.Responses;

public class TarefaResponse
{
  public int Id { get; set; }
  public string Titulo { get; set; } = null!;
  public string? Descricao { get; set; }
  public DateOnly DataCriada { get; set; }
  public DateOnly Prazo { get; set; }
  public PrioridadeTarefa Prioridade { get; set; }
  public StatusTarefa Status { get; set; }

}