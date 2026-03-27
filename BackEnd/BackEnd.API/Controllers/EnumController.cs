using BackEnd.API.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using BackEnd.Dominio.Enumeradores;

namespace BackEnd.API;

[ApiController]
[Route("api/[controller]")]
public class EnumController : ControllerBase
{
  [HttpGet("ListarUsuarioFuncao")]
  public IActionResult ListarUsuarioFuncao()
  {
    try
    {
      /* 
      Enum.GetValues(typeof(UsuarioFuncao)) 
      pega todos os valores do enum como uma coleção. Retorna [1, 2, 3] basicamente.
      */
      var listaUsuarioFuncao = Enum.GetValues(typeof(UsuarioFuncao))
      
      
      /*.Cast<UsuarioFuncao>() — converte cada valor para o tipo UsuarioFuncao.
      Sem isso seria uma coleção de object, não de UsuarioFuncao.*/
      .Cast<UsuarioFuncao>()
      
      
      /*.Select(usuario => ...) — percorre cada valor e cria um UsuarioFuncaoResponse com Id e Nome.*/
      .Select(usuario => new UsuarioFuncaoResponse()
      {
        Id = (int)usuario,
        Nome = usuario.ToString()
      }).ToList();

      return Ok(listaUsuarioFuncao);
    }
    catch (Exception ex)
    {
      var inner = ex.InnerException?.Message;
      return BadRequest(new { message = ex.Message, inner });
    }
  }



  [HttpGet("ListarStatusTarefa")]
  public IActionResult ListarStatusTarefa()
  {
    try
    {
      var listaStatusTarefa = Enum.GetValues(typeof(StatusTarefa))
      .Cast<StatusTarefa>()
      .Select(tarefa => new StatusTarefaResponse()
      {
        Id = (int)tarefa,
        Nome = tarefa.ToString()
      }).ToList();

      return Ok(listaStatusTarefa);
    }
    catch (Exception ex)
    {
      var inner = ex.InnerException?.Message;
      return BadRequest(new { message = ex.Message, inner });
    }
  }


  [HttpGet("ListarPrioridadeTarefa")]
  public IActionResult ListarPrioridadeTarefa()
  {
    try
    {
      var listaPrioridadeTarefa = Enum.GetValues(typeof(PrioridadeTarefa))
      .Cast<PrioridadeTarefa>()
      .Select(tarefa => new PrioridadeTarefaResponse()
      {
        Id = (int)tarefa,
        Nome = tarefa.ToString()
      }).ToList();

      return Ok(listaPrioridadeTarefa);
    }
    catch (Exception ex)
    {
      var inner = ex.InnerException?.Message;
      return BadRequest(new { message = ex.Message, inner });
    }
  }
}