using BackEnd.API.Models.Responses;
using BackEnd.Application;
using BackEnd.Application.DTOs;
using BackEnd.Dominio.Entidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.API;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RelatorioController : ControllerBase
{
  private readonly IRelatorioApplication _relatorioApplication;

  public RelatorioController(IRelatorioApplication relatorioApplication)
  {
    _relatorioApplication = relatorioApplication;
  }

  [HttpGet("ResumoTarefas")]
  public async Task<ActionResult> ObterResumoTarefas()
  {
    try
    {
      // Aqui basicamente nos só esperamos o nosso metodo ObterResumo, depois mostramos
      var resumo = await _relatorioApplication.ObterResumoTarefasAsync();
      return Ok(resumo);
    }
    catch (Exception ex)
    {
      return BadRequest(new { message = ex.Message });
    }
  }

  [HttpGet("TarefasRecentes")]
  public async Task<ActionResult> ObterTarefasRecentes()
  {
    try
    {
      // Armazenamos os dados que encontrarmos atraves do repositories
      var tarefas = await _relatorioApplication.ObterTarefasRecentesAsync();

      // Para cada tarefa que foi encontrada nos passamos o response dela para gente
      var tarefasResponse = tarefas.Select(t => new TarefaResponse
      {
        Id = t.Id,
        Titulo = t.Titulo,
        Descricao = t.Descricao,
        DataCriada = t.DataCriada,
        Prazo = t.Prazo,
        Prioridade = t.Prioridade,
        Status = t.Status
      }).ToList();

      return Ok(tarefasResponse);
    }
    catch (Exception ex)
    {
      return BadRequest(new { message = ex.Message });
    }
  }

  [HttpGet("ProgressoGeral")]
  public async Task<ActionResult> ObterProgressoGeral()
  {
    try
    {
      // So buscamos e trazemos o DOUBLE para montarmos a nossa barra de busca
      var progresso = await _relatorioApplication.ObterProgressoGeralAsync();
      return Ok(new { porcentagem = progresso });
    }
    catch (Exception ex)
    {
      return BadRequest(new { message = ex.Message });
    }
  }

  [HttpGet("ResumoEquipe")]
  public async Task<ActionResult> ObterResumoEquipe()
  {
    try
    {
      // So recebemos os dados do nosso Application
      var equipe = await _relatorioApplication.ObterResumoEquipeAsync();
      return Ok(equipe);
    }
    catch (Exception ex)
    {
      return BadRequest(new { message = ex.Message });
    }
  }
}