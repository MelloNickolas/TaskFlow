using BackEnd.Dominio.Entidades;
using BackEnd.API.Models.Requests;
using BackEnd.API.Models.Responses;
using BackEnd.Application;
using Microsoft.AspNetCore.Mvc;
using BackEnd.Dominio.Enumeradores;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace BackEnd.API;

[ApiController]
[Route("api/[controller]")]
public class TarefaController : ControllerBase
{
  private readonly ITarefaApplication _tarefaApplication;
  /* Injetando dependencias para verificar se a tarefa esta atribuida para aquele desenvolvedor, permitindo que ele possa alterar o status da tarefa*/
  private readonly IUsuarioTarefaApplication _usuarioTarefaApplication;
  public TarefaController(ITarefaApplication tarefaApplication, IUsuarioTarefaApplication usuarioTarefaApplication)
  {
    _tarefaApplication = tarefaApplication;
    _usuarioTarefaApplication = usuarioTarefaApplication;
  }

  [Authorize(Roles = "Administrador, Gerente")]
  [HttpPost("CriarTarefa")]
  public async Task<ActionResult> CriarTarefa([FromBody] TarefaCriar tarefaCriar)
  {
    try
    {
      // Criamos uma instancia do nosso dominio
      var tarefaDominio = new Tarefa();

      // Aqui definimos os método que criamos lá no dominio, fizemos assim pq temos vários métodos private para encapsulamento
      // mas poderiamos colocar direto caso fosse público
      tarefaDominio.DefinirTitulo(tarefaCriar.Titulo);
      tarefaDominio.DefinirDescricao(tarefaCriar.Descricao);
      tarefaDominio.DefinirPrazo(tarefaCriar.Prazo);
      tarefaDominio.DefinirPrioridade(tarefaCriar.Prioridade);

      // Usamos o método criar da aplicaçao com um await pois estamos pegando dados assincronos e passamos os paramtretos do método
      var IdTarefa = await _tarefaApplication.CriarAsync(tarefaDominio);

      // Retornamos com açao concluida
      return Ok(IdTarefa);
    }
    catch (Exception ex)
    {
      var inner = ex.InnerException?.Message;
      return BadRequest(new { message = ex.Message, inner });
    }
  }


  [Authorize]
  [HttpGet("ObterTarefaPorId/{IdTarefa}")]
  public async Task<ActionResult> ObterTarefaPorId([FromRoute] int IdTarefa)
  {
    try
    {
      // Obtemos o usuario pelo id no nosso dominio pela aplicaçao
      var tarefaDominio = await _tarefaApplication.ObterPorIdAsync(IdTarefa);

      // Definimos que a resposta vai ser que nem o usuario que achamos por ID
      var tarefaResponse = new TarefaResponse()
      {
        Id = tarefaDominio.Id,
        Titulo = tarefaDominio.Titulo,
        Descricao = tarefaDominio.Descricao,
        DataCriada = tarefaDominio.DataCriada,
        Prazo = tarefaDominio.Prazo,
        Prioridade = tarefaDominio.Prioridade,
        Status = tarefaDominio.Status
      };

      return Ok(tarefaResponse);
    }
    catch (Exception ex)
    {
      var inner = ex.InnerException?.Message;
      return BadRequest(new { message = ex.Message, inner });
    }
  }


  [Authorize(Roles = "Administrador, Gerente")]
  [HttpPut("AtualizarTarefa")]
  public async Task<ActionResult> AtualizarTarefa([FromBody] TarefaAtualizar tarefaAtualizar)
  {
    try
    {
      // Buscamos o usuario do dominio pelo Id que foi passado
      var tarefaDominio = await _tarefaApplication.ObterPorIdAsync(tarefaAtualizar.Id);

      // Métodos do dominio para a atualizaçao
      tarefaDominio.DefinirTitulo(tarefaAtualizar.Titulo);
      tarefaDominio.DefinirDescricao(tarefaAtualizar.Descricao);
      tarefaDominio.DefinirPrazo(tarefaAtualizar.Prazo);
      tarefaDominio.DefinirPrioridade(tarefaAtualizar.Prioridade);
      tarefaDominio.DefinirStatus(tarefaAtualizar.Status);

      var tarefaResponse = new TarefaResponse()
      {
        Id = tarefaDominio.Id,
        Titulo = tarefaDominio.Titulo,
        Descricao = tarefaDominio.Descricao,
        DataCriada = tarefaDominio.DataCriada,
        Prazo = tarefaDominio.Prazo,
        Prioridade = tarefaDominio.Prioridade,
        Status = tarefaDominio.Status
      };


      // Chamamos o método da nossa aplicacao para podermos atualizar tudo isso
      await _tarefaApplication.AtualizarAsync(tarefaDominio);
      return Ok(tarefaResponse);
    }
    catch (Exception ex)
    {
      var inner = ex.InnerException?.Message;
      return BadRequest(new { message = ex.Message, inner });
    }
  }




  [Authorize]
  [HttpPut("AtualizarStatusTarefa")]
  public async Task<ActionResult> AtualizarStatusTarefa([FromBody] TarefaAtualizarStatus tarefaAtualizarStatus)
  {
    try
    {
      // busca o id do usuario logado
      var idUsuarioLogado = User.FindFirst("id")?.Value;

      // Verifica se a tarefa está atribuída ao usuário logado
      var relacionamentoTarefa = await _usuarioTarefaApplication.ObterPorTarefaAsync(tarefaAtualizarStatus.Id);

      var tarefaAtribuida = relacionamentoTarefa?.Any(ut => ut.UsuarioId.ToString() == idUsuarioLogado);

      if (tarefaAtribuida != true)
        return Forbid();

      // Buscamos o usuario do dominio pelo Id que foi passado
      var tarefaDominio = await _tarefaApplication.ObterPorIdAsync(tarefaAtualizarStatus.Id);


      tarefaDominio.DefinirStatus(tarefaAtualizarStatus.Status);

      var tarefaResponse = new TarefaResponse()
      {
        Id = tarefaDominio.Id,
        Titulo = tarefaDominio.Titulo,
        Descricao = tarefaDominio.Descricao,
        DataCriada = tarefaDominio.DataCriada,
        Prazo = tarefaDominio.Prazo,
        Prioridade = tarefaDominio.Prioridade,
        Status = tarefaDominio.Status
      };


      // Chamamos o método da nossa aplicacao para podermos atualizar tudo isso
      await _tarefaApplication.AtualizarAsync(tarefaDominio);
      return Ok(tarefaResponse);
    }
    catch (Exception ex)
    {
      var inner = ex.InnerException?.Message;
      return BadRequest(new { message = ex.Message, inner });
    }
  }




  [Authorize(Roles = "Administrador, Gerente")]
  [HttpDelete("DeletarTarefa/{IdTarefa}")]
  public async Task<ActionResult> DeletarTarefa([FromRoute] int IdTarefa)
  {
    try
    {
      await _tarefaApplication.DeletarAsync(IdTarefa);
      return Ok();
    }
    catch (Exception ex)
    {
      var inner = ex.InnerException?.Message;
      return BadRequest(new { message = ex.Message, inner });
    }
  }


  [Authorize]
  [HttpGet("ListarTarefas")]
  public async Task<ActionResult> ListarTarefas()
  {
    try
    {
      // Puxamos uma lista de usuarios dos dominios atraves do metodo
      var tarefaDominio = await _tarefaApplication.ListarAsync();

      // Se a lista estiver vazia retorna uma lista vazia em vez de dar erro
      if (tarefaDominio == null)
        return Ok(new List<TarefaResponse>());

      // Aqui vamos percorrer cada um deles dando um select para mostrar todos os usuarios e as infos deles.
      var tarefasLista = tarefaDominio.Select(x => new TarefaResponse()
      {
        Id = x.Id,
        Titulo = x.Titulo,
        Descricao = x.Descricao,
        DataCriada = x.DataCriada,
        Prazo = x.Prazo,
        Prioridade = x.Prioridade,
        Status = x.Status

      }).ToList();

      return Ok(tarefasLista);
    }
    catch (Exception ex)
    {
      var inner = ex.InnerException?.Message;
      return BadRequest(new { message = ex.Message, inner });
    }
  }


  [Authorize]
  [HttpGet("ListarTarefasPorStatus")]
  public async Task<ActionResult> ListarTarefasPorStatus([FromQuery] StatusTarefa statusTarefa)
  {
    try
    {
      // Puxamos uma lista de usuarios dos dominios atraves do metodo
      var tarefaDominio = await _tarefaApplication.ListarPorStatusAsync(statusTarefa);

      // Se a lista estiver vazia retorna uma lista vazia em vez de dar erro
      if (tarefaDominio == null)
        return Ok(new List<TarefaResponse>());

      // Aqui vamos percorrer cada um deles dando um select para mostrar todos os usuarios e as infos deles.
      var tarefasLista = tarefaDominio.Select(x => new TarefaResponse()
      {
        Id = x.Id,
        Titulo = x.Titulo,
        Descricao = x.Descricao,
        DataCriada = x.DataCriada,
        Prazo = x.Prazo,
        Prioridade = x.Prioridade,
        Status = x.Status

      }).ToList();

      return Ok(tarefasLista);
    }
    catch (Exception ex)
    {
      var inner = ex.InnerException?.Message;
      return BadRequest(new { message = ex.Message, inner });
    }
  }


  [Authorize]
  [HttpGet("ListarTarefasPorPrioridade")]
  public async Task<ActionResult> ListarTarefasPorPrioridade([FromQuery] PrioridadeTarefa prioridadeTarefa)
  {
    try
    {
      // Puxamos uma lista de usuarios dos dominios atraves do metodo
      var tarefaDominio = await _tarefaApplication.ListarPorPrioridadeAsync(prioridadeTarefa);

      // Se a lista estiver vazia retorna uma lista vazia em vez de dar erro
      if (tarefaDominio == null)
        return Ok(new List<TarefaResponse>());

      // Aqui vamos percorrer cada um deles dando um select para mostrar todos os usuarios e as infos deles.
      var tarefasLista = tarefaDominio.Select(x => new TarefaResponse()
      {
        Id = x.Id,
        Titulo = x.Titulo,
        Descricao = x.Descricao,
        DataCriada = x.DataCriada,
        Prazo = x.Prazo,
        Prioridade = x.Prioridade,
        Status = x.Status

      }).ToList();

      return Ok(tarefasLista);
    }
    catch (Exception ex)
    {
      var inner = ex.InnerException?.Message;
      return BadRequest(new { message = ex.Message, inner });
    }
  }


  [Authorize]
  [HttpGet("ListarTarefasPorUsuario")]
  public async Task<ActionResult> ListarTarefasPorUsuario([FromQuery] int IdUsuario)
  {
    try
    {
      // Puxamos uma lista de usuarios dos dominios atraves do metodo
      var tarefaDominio = await _tarefaApplication.ListarPorUsuarioAsync(IdUsuario);

      // Se a lista estiver vazia retorna uma lista vazia em vez de dar erro
      if (tarefaDominio == null)
        return Ok(new List<TarefaResponse>());

      // Aqui vamos percorrer cada um deles dando um select para mostrar todos os usuarios e as infos deles.
      var tarefasLista = tarefaDominio.Select(x => new TarefaResponse()
      {
        Id = x.Id,
        Titulo = x.Titulo,
        Descricao = x.Descricao,
        DataCriada = x.DataCriada,
        Prazo = x.Prazo,
        Prioridade = x.Prioridade,
        Status = x.Status

      }).ToList();

      return Ok(tarefasLista);
    }
    catch (Exception ex)
    {
      var inner = ex.InnerException?.Message;
      return BadRequest(new { message = ex.Message, inner });
    }
  }

}