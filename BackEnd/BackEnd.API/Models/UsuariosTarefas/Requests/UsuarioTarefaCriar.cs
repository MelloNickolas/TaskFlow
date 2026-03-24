using System.ComponentModel.DataAnnotations;

namespace BackEnd.API.Models.Requests;

public class UsuarioTarefaCriar
{
  [Required(ErrorMessage = "O campo USUARIOID é obrigatório.")]
  public int UsuarioId { get; set; }

  [Required(ErrorMessage = "O campo TAREFAID é obrigatório.")]
  public int TarefaId { get; set; }

}