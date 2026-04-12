using BackEnd.Dominio.Entidades;
using BackEnd.API.Models.Requests;
using BackEnd.API.Models.Responses;
using BackEnd.Application;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace BackEnd.API;

/*
  [ApiController] — sinaliza que é um controller de API e habilita funcionalidades automáticas como validação dos [Required] que você colocou nos models.

  [Route("[controller]")] — o [controller] é substituído automaticamente pelo nome da classe sem o sufixo "Controller". Então UsuarioController vira a rota /Usuario

  ControllerBase — fornece métodos como Ok(), NotFound(), BadRequest() para retornar respostas HTTP.
*/
[ApiController]
[Route("api/[controller]")]
public class UsuarioController : ControllerBase
{
  /*Injetando dependencias para podermos usar os nossos métodos da Application */
  private readonly IUsuarioApplication _usuarioApplication;
  public UsuarioController(IUsuarioApplication usuarioApplication)
  {
    _usuarioApplication = usuarioApplication;
  }

  /*
  Aqui ele já vai chamar o método post e colocar CriarUsuario direto na nossa url da API 
  Estamos definindo qual método usaremos e qual rota ele vai pertencer.

  async Task<ActionResult> — o método é assíncrono e retorna uma resposta HTTP como Ok(), BadRequest(), NotFound().

  [FromBody] — diz ao .NET que os dados vêm no corpo da requisição HTTP em formato JSON. Quando o frontend manda 
  um POST com JSON, o .NET deserializa automaticamente para o objeto UsuarioCriar.
  [FromRoute] — pega da URL, ex: /api/usuario/1
  [FromQuery] — pega da query string, ex: /api/usuario?ativo=true
  */

  /*De acordo com nosso Token, vamos verificar, e se não for do tipo Administrador, não tem permissão para criar usuário*/
  [Authorize(Roles = "Administrador")]
  [HttpPost("CriarUsuario")]
  public async Task<ActionResult> CriarUsuario([FromBody] UsuarioCriar usuarioCriar)
  {
    try
    {
      // Criamos uma instancia do nosso dominio
      var usuarioDominio = new Usuario();

      // Aqui definimos os método que criamos lá no dominio, fizemos assim pq temos vários métodos private para encapsulamento
      // mas poderiamos colocar direto caso fosse público
      usuarioDominio.DefinirNome(usuarioCriar.Nome);
      usuarioDominio.DefinirEmail(usuarioCriar.Email);
      usuarioDominio.DefinirFuncao(usuarioCriar.Funcao);

      // Usamos o método criar da aplicaçao com um await pois estamos pegando dados assincronos e passamos os paramtretos do método
      var IdUsuario = await _usuarioApplication.CriarAsync(usuarioDominio, usuarioCriar.Senha);

      // Retornamos com açao concluida
      return Ok(IdUsuario);
    }
    catch (Exception ex)
    {
      var inner = ex.InnerException?.Message;
      return BadRequest(new { message = ex.Message, inner });
    }
  }





  [Authorize]
  [HttpGet("ObterUsuarioPorId/{IdUsuario}")]
  public async Task<ActionResult> ObterUsuarioPorId([FromRoute] int IdUsuario)
  {
    try
    {
      // Obtemos o usuario pelo id no nosso dominio pela aplicaçao
      var usuarioDominio = await _usuarioApplication.ObterPorIdAsync(IdUsuario);

      // Definimos que a resposta vai ser que nem o usuario que achamos por ID
      var usuarioResponse = new UsuarioResponse()
      {
        Id = usuarioDominio.Id,
        Nome = usuarioDominio.Nome,
        Email = usuarioDominio.Email,
        Funcao = usuarioDominio.Funcao
      };

      return Ok(usuarioResponse);
    }
    catch (Exception ex)
    {
      var inner = ex.InnerException?.Message;
      return BadRequest(new { message = ex.Message, inner });
    }
  }


  [Authorize(Roles = "Administrador, Gerente")]
  [HttpPut("AtualizarUsuario")]
  public async Task<ActionResult> AtualizarUsuario([FromBody] UsuarioAtualizar usuarioAtualizar)
  {
    try
    {
      // Buscamos o usuario do dominio pelo Id que foi passado
      var usuarioDominio = await _usuarioApplication.ObterPorIdAsync(usuarioAtualizar.Id);


      // Métodos do dominio para a atualizaçao
      usuarioDominio.DefinirNome(usuarioAtualizar.Nome);
      usuarioDominio.DefinirEmail(usuarioAtualizar.Email);
      usuarioDominio.DefinirFuncao(usuarioAtualizar.Funcao);

      var usuarioResponse = new UsuarioResponse()
      {
        Id = usuarioDominio.Id,
        Nome = usuarioDominio.Nome,
        Email = usuarioDominio.Email,
        Funcao = usuarioDominio.Funcao
      };

      // Chamamos o método da nossa aplicacao para podermos atualizar tudo isso
      await _usuarioApplication.AtualizarAsync(usuarioDominio);
      return Ok(usuarioResponse);
    }
    catch (Exception ex)
    {
      var inner = ex.InnerException?.Message;
      return BadRequest(new { message = ex.Message, inner });
    }
  }


  [Authorize]
  [HttpPut("AtualizarSenhaUsuario")]
  public async Task<ActionResult> AtualizarSenhaUsuario([FromBody] UsuarioAtualizarSenha usuarioAtualizarSenha)
  {
    try
    {
      // Pegamos o Id do usuário logado pelo token
      var idUsuarioLogado = User.FindFirst("id")?.Value;
      // Se não for Administrador, só pode alterar a própria senha
      if(!User.IsInRole("Administrador") && idUsuarioLogado != usuarioAtualizarSenha.Id.ToString())
        return Forbid(); // usuario esta autenticado mas nao tem permissao!

      // Buscamos o usuario do dominio pelo Id que foi passado
      var usuarioDominio = await _usuarioApplication.ObterPorIdAsync(usuarioAtualizarSenha.Id);


      // Chamamos o método da nossa aplicacao para podermos atualizar tudo isso
      await _usuarioApplication.AtualizarSenhaAsync(usuarioDominio, usuarioAtualizarSenha.senhaAntiga, usuarioAtualizarSenha.senhaNova);
      return Ok();
    }
    catch (Exception ex)
    {
      var inner = ex.InnerException?.Message;
      return BadRequest(new { message = ex.Message, inner });
    }
  }

  [Authorize(Roles = "Administrador")]
  [HttpDelete("DeletarUsuario/{IdUsuario}")]
  public async Task<ActionResult> DeletarUsuario([FromRoute] int IdUsuario)
  {
    try
    {
      // Chamamos o método da nossa aplicacao para podermos atualizar tudo isso
      await _usuarioApplication.DeletarAsync(IdUsuario);

      return Ok();
    }
    catch (Exception ex)
    {
      var inner = ex.InnerException?.Message;
      return BadRequest(new { message = ex.Message, inner });
    }
  }


  [Authorize(Roles = "Administrador")]
  [HttpPut("RestaurarUsuario/{IdUsuario}")]
  public async Task<ActionResult> RestaurarUsuario([FromRoute] int IdUsuario)
  {
    try
    {
      // Chamamos o método da nossa aplicacao para podermos atualizar tudo isso
      await _usuarioApplication.RestaurarAsync(IdUsuario);

      return Ok();
    }
    catch (Exception ex)
    {
      var inner = ex.InnerException?.Message;
      return BadRequest(new { message = ex.Message, inner });
    }
  }


  [Authorize]
  [HttpGet("ListarUsuarios")]
  public async Task<ActionResult> ListarUsuarios([FromQuery] bool ativo)
  {
    try
    {
      // Puxamos uma lista de usuarios dos dominios atraves do metodo
      var usuarioDominio = await _usuarioApplication.ListarAsync(ativo);

      // Se a lista estiver vazia retorna uma lista vazia em vez de dar erro
      if (usuarioDominio == null)
        return Ok(new List<UsuarioResponse>());

      // Aqui vamos percorrer cada um deles dando um select para mostrar todos os usuarios e as infos deles.
      var usuariosLista = usuarioDominio.Select(x => new UsuarioResponse()
      {
        Id = x.Id,
        Nome = x.Nome,
        Email = x.Email,
        Funcao = x.Funcao
      }).ToList();

      return Ok(usuariosLista);
    }
    catch (Exception ex)
    {
      var inner = ex.InnerException?.Message;
      return BadRequest(new { message = ex.Message, inner });
    }
  }

}