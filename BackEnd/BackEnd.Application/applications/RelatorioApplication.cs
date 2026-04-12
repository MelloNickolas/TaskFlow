using BackEnd.Application.DTOs;
using BackEnd.Dominio.Entidades;
using BackEnd.Dominio.Enumeradores;
using Microsoft.AspNetCore.Authorization;

namespace BackEnd.Application;

public class RelatorioApplication : IRelatorioApplication
{
  private readonly ITarefaRepository _tarefaRepository;
  private readonly IUsuarioRepository _usuarioRepository;

  public RelatorioApplication(ITarefaRepository tarefaRepository, IUsuarioRepository usuarioRepository)
  {
    _tarefaRepository = tarefaRepository;
    _usuarioRepository = usuarioRepository;
  }

  public async Task<double> ObterProgressoGeralAsync()
  {
    /* Listamos todas as tarefas que existem */
    var todasTarefas = await _tarefaRepository.ListarAsync();

    /* Caso nao tenha nehuma tarefa retorna um DTO vazio */
    if (todasTarefas == null)
      return 0;

    /* caso tenho tarefas transforma os dados que pegamos em uma lista. */
    var lista = todasTarefas.ToList();

    var porcentagem = ((double)lista.Count(t => t.Status == StatusTarefa.Concluido) / lista.Count()) * 100;

    return porcentagem;
  }

  public async Task<IEnumerable<ResumoEquipeDTO>> ObterResumoEquipeAsync()
  {
    // Trago todos os usuarios que estão ativos
    var todosUsuarios = await _usuarioRepository.ListarAsync(true);

    // Se não tiver um usuario retorna uma lista vazia
    if (todosUsuarios == null)
      return new List<ResumoEquipeDTO>();

    // Sse tiver vai transformar todos os usuarios em uma lista
    var lista = todosUsuarios.ToList();

    // para cada usuario dentro dessa lista nos vamos passar o DTO, para os dados
    // que precisa mos no nosso relatorio
    var listaEquipe = lista.Select(u => new ResumoEquipeDTO
    {
      UsuarioId = u.Id,
      Nome = u.Nome,
      Funcao = u.Funcao.ToString(),
      // Quantas atribuiçoes tem??
      TotalTarefas = u.UsuarioTarefas?.Count ?? 0 // se for nulo ele traz 0
    }).ToList();

    var listaMaisAtribuicoes = listaEquipe.OrderByDescending(u => u.TotalTarefas).Take(5).ToList();

    return listaMaisAtribuicoes;
  }


  public async Task<ResumoTarefasDTO> ObterResumoTarefasAsync()
  {
    /* Listamos todas as tarefas que existem */
    var todasTarefas = await _tarefaRepository.ListarAsync();

    /* Caso nao tenha nehuma tarefa retorna um DTO vazio */
    if (todasTarefas == null)
      return new ResumoTarefasDTO();

    /* caso tenho tarefas transforma os dados que pegamos em uma lista. */
    var lista = todasTarefas.ToList();

    /* Aqui vamos basicamente pegar o inicio da semana, se estamos na terça ele vai buscar terça passada*/
    var inicioSemana = DateOnly.FromDateTime(DateTime.Today.AddDays(-7));

    /* Ja aplicamos um filtro para trazer somente a lista com as tarefas desse periodo */
    var listaSemana = lista.Where(t => t.DataCriada >= inicioSemana).ToList();

    /* Aqui vamos relacionar com o nosso DTO e passar as infos */
    return new ResumoTarefasDTO
    {
      TotalTarefas = lista.Count,
      TarefasConcluidas = lista.Count(t => t.Status == StatusTarefa.Concluido),
      TarefasEmAndamento = lista.Count(t => t.Status == StatusTarefa.EmAndamento),
      TarefasAFazer = lista.Count(t => t.Status == StatusTarefa.AFazer),
      TotalEstaSemana = listaSemana.Count,
      TarefasConcluidasEstaSemana = listaSemana.Count(t => t.Status == StatusTarefa.Concluido),
      TarefasEmAndamentoEstaSemana = listaSemana.Count(t => t.Status == StatusTarefa.EmAndamento),
      TarefasAFazerEstaSemana = listaSemana.Count(t => t.Status == StatusTarefa.AFazer)
    };
  }


  public async Task<IEnumerable<Tarefa>> ObterTarefasRecentesAsync()
  {
    /* Listamos todas as tarefas que existem */
    var todasTarefas = await _tarefaRepository.ListarAsync();

    /* Caso nao tenha nehuma tarefa retorna um DTO vazio */
    if (todasTarefas == null)
      return new List<Tarefa>();

    /* caso tenho tarefas transforma os dados que pegamos em uma lista. */
    var lista = todasTarefas.ToList();

    /* retorna as 5 tarefas mais recentes */
    var listaRecentes = lista.OrderByDescending(t => t.DataCriada).Take(5).ToList();

    return listaRecentes;
  }
}