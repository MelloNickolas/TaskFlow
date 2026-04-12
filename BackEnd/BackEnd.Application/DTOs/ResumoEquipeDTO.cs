namespace BackEnd.Application.DTOs;

public class ResumoEquipeDTO
{
  /* Puxamos o Id do usuario para ver as informações */
  public int UsuarioId { get; set; }
  public string Nome { get; set; } = null!;

  /* Nos retornamos uma string pois acredito que passa o nome da função, é melhor que passar o ID. */
  public string Funcao { get; set; } = null!;
  public int TotalTarefas { get; set; }
}