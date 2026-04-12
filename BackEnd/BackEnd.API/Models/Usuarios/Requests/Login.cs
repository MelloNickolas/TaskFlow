using System.ComponentModel.DataAnnotations;

namespace BackEnd.API.Models.Requests;

public class Login
{
  [Required(ErrorMessage = "O campo EMAIL é obrigatório.")]
  [EmailAddress(ErrorMessage = "O formato do e-mail não é válido.")]

  public string Email { get; set; } = null!;
  
  [Required(ErrorMessage = "O campo SENHA é obrigatório.")]
  public string Senha { get; set; } = null!;
}