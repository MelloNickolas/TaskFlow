using BackEnd.Dominio.Enumeradores;

namespace BackEnd.API.Models.Responses;

public class UsuarioResponse
{
  public int Id { get; set; }
  public string Nome { get; set; } = null!;
  public string Email { get; set; } = null!;
  public UsuarioFuncao Funcao { get; set; }

}