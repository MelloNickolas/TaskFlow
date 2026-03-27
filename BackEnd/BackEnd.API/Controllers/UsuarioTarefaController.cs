using BackEnd.Dominio.Entidades;
using BackEnd.API.Models.Requests;
using BackEnd.API.Models.Responses;
using BackEnd.Application;
using Microsoft.AspNetCore.Mvc;
using BackEnd.Dominio.Enumeradores;

namespace BackEnd.API;

[ApiController]
[Route("api/[controller]")]
public class UsuarioTarefaController : ControllerBase
{
  private readonly IUsuarioTarefaApplication _usuarioTarefaApplication;
  public UsuarioTarefaController(IUsuarioTarefaApplication usuarioTarefaApplication)
  {
    _usuarioTarefaApplication = usuarioTarefaApplication;
  }

    [HttpPost("CriarRelacionamento")]
    public async Task<ActionResult> CriarRelacionamento([FromBody] UsuarioTarefaCriar usuarioTarefaCriar)
    {
      try
      {
        var usuarioTarefaDominio = new UsuarioTarefa(usuarioTarefaCriar.UsuarioId, usuarioTarefaCriar.TarefaId);
        
        var IdUsuarioTarefa = await _usuarioTarefaApplication.SalvarAsync(usuarioTarefaDominio);

        return Ok(IdUsuarioTarefa);
      }
      catch (Exception ex)
      {
        var inner = ex.InnerException?.Message;
        return BadRequest(new { message = ex.Message, inner });
      }
    }



  [HttpGet("ObterRelacionamentoPorId/{Id}")]
  public async Task<ActionResult> ObterRelacionamentoPorId([FromRoute] int Id)
  {
    try
    {
      // Obtemos o usuario pelo id no nosso dominio pela aplicaçao
      var usuarioTarefaDominio = await _usuarioTarefaApplication.ObterPorIdAsync(Id);

      // Definimos que a resposta vai ser que nem o usuario que achamos por ID
      var usuarioTarefaResponse = new UsuarioTarefaResponse()
      {
        Id = usuarioTarefaDominio.Id,
        UsuarioId = usuarioTarefaDominio.UsuarioId,
        TarefaId = usuarioTarefaDominio.TarefaId
      };

      return Ok(usuarioTarefaResponse);
    }
    catch (Exception ex)
    {
      var inner = ex.InnerException?.Message;
      return BadRequest(new { message = ex.Message, inner });
    }
  }

  [HttpDelete("DeletarRelacionamento/{Id}")]
  public async Task<ActionResult> DeletarRelacionamento([FromRoute] int Id)
  {
    try
    {
      await _usuarioTarefaApplication.DeletarAsync(Id);
      return Ok();
    }
    catch (Exception ex)
    {
      var inner = ex.InnerException?.Message;
      return BadRequest(new { message = ex.Message, inner });
    }
  }



  [HttpGet("ObterRelacionamentoPorUsuario")]
  public async Task<ActionResult> ObterRelacionamentoPorUsuario([FromQuery] int usuarioId)
  {
    try
    {
      // Obtemos o usuario pelo id no nosso dominio pela aplicaçao
      var usuarioTarefaDominio = await _usuarioTarefaApplication.ObterPorUsuarioAsync(usuarioId);
      if (usuarioTarefaDominio == null)
        return Ok(new List<UsuarioTarefaResponse>());

      // Definimos que a resposta vai ser que nem o usuario que achamos por ID
      var usuarioTarefaLista = usuarioTarefaDominio.Select(relacionamento => new UsuarioTarefaResponse()
      {
        Id = relacionamento.Id,
        UsuarioId = relacionamento.UsuarioId,
        TarefaId = relacionamento.TarefaId
      }).ToList();

      return Ok(usuarioTarefaLista);
    }
    catch (Exception ex)
    {
      var inner = ex.InnerException?.Message;
      return BadRequest(new { message = ex.Message, inner });
    }
  }


  [HttpGet("ObterRelacionamentoPorTarefa")]
  public async Task<ActionResult> ObterRelacionamentoPorTarefa([FromQuery] int tarefaId)
  {
    try
    {
      // Obtemos o usuario pelo id no nosso dominio pela aplicaçao
      var usuarioTarefaDominio = await _usuarioTarefaApplication.ObterPorTarefaAsync(tarefaId);
      if (usuarioTarefaDominio == null)
        return Ok(new List<UsuarioTarefaResponse>());

      // Definimos que a resposta vai ser que nem o usuario que achamos por ID
      var usuarioTarefaLista = usuarioTarefaDominio.Select(relacionamento => new UsuarioTarefaResponse()
      {
        Id = relacionamento.Id,
        UsuarioId = relacionamento.UsuarioId,
        TarefaId = relacionamento.TarefaId
      }).ToList();

      return Ok(usuarioTarefaLista);
    }
    catch (Exception ex)
    {
      var inner = ex.InnerException?.Message;
      return BadRequest(new { message = ex.Message, inner });
    }
  }

}