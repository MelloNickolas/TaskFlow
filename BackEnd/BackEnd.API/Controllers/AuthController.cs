using BackEnd.API.Models.Requests;
using BackEnd.Application;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.API;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
  /* Injetamos o usuario para podermos usar os métodos da aplicação dele */
  private readonly IUsuarioApplication _usuarioApplication;

  public AuthController(IUsuarioApplication usuarioApplication)
  {
    _usuarioApplication = usuarioApplication;
  }

  /*
  Ele vai solicitar o nosso model Request Login, e vai pegar o corpo dessa request
  Com isso ele vai criar o toquen passando esse email e senha, aguardando a validação do Application
  Se der certo ele retorna o token para usarmos
  */
  [HttpPost("Login")]
  public async Task<ActionResult> Login([FromBody] Login login)
  {
    try
    {
      var token = await _usuarioApplication.LoginAsync(login.Email, login.Senha);
      return Ok(new { token });
    }
    catch (Exception ex)
    {
      var inner = ex.InnerException?.Message;
      return BadRequest(new { message = ex.Message, inner });
    }
  }
}