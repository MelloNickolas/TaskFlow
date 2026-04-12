using BackEnd.Dominio.Entidades;
using BackEnd.Application.DTOs;

namespace BackEnd.Application;

public interface IRelatorioApplication
{
  /* 
 O problema é que os models ficam na camada BackEnd.API e a Application não referencia a API — e não deve! Seria uma dependência circular.
  A solução é criar os responses na camada de Application mesmo, numa pasta Models ou DTOs. Assim a API usa os responses da Application.

  Por isso criamos os DTOs para realizar essa criaçao e não infringir na nossa regra de negocio
  */

  /* Aqui vamos pegar o nosso dto que ja tem os campos necessários para passar ao endpoint*/
  Task<ResumoTarefasDTO> ObterResumoTarefasAsync();

  /* Aqui vamos pegar as tarefas que temos, e filtrar pelas mais recentes s*/
  Task<IEnumerable<Tarefa>> ObterTarefasRecentesAsync();

  /* aqui vamos retornar um numero com virgula pois será uma porcentagem */
  Task<double> ObterProgressoGeralAsync();

  /* Aqui vamos buscar o usuario a funcao dele e retornar o total de tarefas que temos atribuido a ele*/
  Task<IEnumerable<ResumoEquipeDTO>> ObterResumoEquipeAsync();
}