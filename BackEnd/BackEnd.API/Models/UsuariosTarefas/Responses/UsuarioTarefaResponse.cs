using BackEnd.Dominio.Enumeradores;

namespace BackEnd.API.Models.Responses;

public class UsuarioTarefaResponse
{
  public int Id { get; set; }
  public int UsuarioId { get; set; }
  public int TarefaId { get; set; }

}