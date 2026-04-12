using System.ComponentModel.DataAnnotations;
using BackEnd.Dominio.Enumeradores;

namespace BackEnd.API.Models.Requests;

/* Feito para atualizar os status das tarefas somente para o desenvolvedor*/
public class TarefaAtualizarStatus
{

  [Required(ErrorMessage = "O campo ID é obrigatório.")]
  public int Id { get; set; }


  [Required(ErrorMessage = "O campo STATUS é obrigatório.")]
  public StatusTarefa Status { get; set; }
}