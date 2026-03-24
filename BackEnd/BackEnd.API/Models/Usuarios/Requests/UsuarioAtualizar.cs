using System.ComponentModel.DataAnnotations;
using BackEnd.Dominio.Enumeradores;


namespace BackEnd.API.Models.Requests;

public class UsuarioAtualizar
{

  /*
  Nós colocamos o Required para mostrar que o campo tem que ser preenchido além de colocar o
  [EmailAddress] que verifica se os dados passados são um email
  = null! - Ele meio que avisa o compilador "pode confiar, eu sei o que estou fazendo, não vai ser null"
  */

  [Required(ErrorMessage = "O ID não pode ser nulo")]
  public int Id { get; set; }

  [Required(ErrorMessage = "O campo NOME é obrigatório.")]
  public string Nome { get; set; } = null!;


  [Required(ErrorMessage = "O campo EMAIL é obrigatório.")]
  [EmailAddress(ErrorMessage = "O formato do e-mail não é válido.")]
  public string Email { get; set; } = null!;


  [Required(ErrorMessage = "O usuário deve possuir uma FUNÇÃO.")]
  public UsuarioFuncao Funcao { get; set; }
}