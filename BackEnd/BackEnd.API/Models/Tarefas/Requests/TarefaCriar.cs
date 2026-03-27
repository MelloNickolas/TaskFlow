using System.ComponentModel.DataAnnotations;
using BackEnd.Dominio.Enumeradores;

namespace BackEnd.API.Models.Requests;

public class TarefaCriar
{

  /*
  Nós colocamos o Required para mostrar que o campo tem que ser preenchido além de colocar o
  [EmailAddress] que verifica se os dados passados são um email
  = null! - Ele meio que avisa o compilador "pode confiar, eu sei o que estou fazendo, não vai ser null"
  */
  [Required(ErrorMessage = "O campo TITULO é obrigatório.")]
  public string Titulo { get; set; } = null!;

  public string? Descricao { get; set; }


  [Required(ErrorMessage = "O campo DATA é obrigatório.")]
  public DateOnly Prazo { get; set; }

  [Required(ErrorMessage = "O campo PRIORIDADE é obrigatório.")]
  public PrioridadeTarefa Prioridade { get; set; }

}