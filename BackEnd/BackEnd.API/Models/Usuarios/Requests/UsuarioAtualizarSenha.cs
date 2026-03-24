using System.ComponentModel.DataAnnotations;
using BackEnd.Dominio.Enumeradores;


namespace BackEnd.API.Models.Requests;

public class UsuarioAtualizarSenha
{

  /*
  Nós colocamos o Required para mostrar que o campo tem que ser preenchido além de colocar o
  [EmailAddress] que verifica se os dados passados são um email
  = null! - Ele meio que avisa o compilador "pode confiar, eu sei o que estou fazendo, não vai ser null"
  */

  [Required(ErrorMessage = "O ID não pode ser nulo")]
  public int Id { get; set; }


  [Required(ErrorMessage = "O campo SENHA ANTIGA é obrigatório.")]
  public string senhaAntiga { get; set; } = null!;

  [Required(ErrorMessage = "O campo SENHA NOVA é obrigatório.")]
  public string senhaNova { get; set; } = null!;

}