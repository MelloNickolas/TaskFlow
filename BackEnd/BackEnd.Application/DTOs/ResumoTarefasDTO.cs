namespace BackEnd.Application.DTOs;

public class ResumoTarefasDTO
{
  // Retorna numeros inteiros pois vai retornar a qtde de cada um.
  public int TotalTarefas { get; set; }
  public int TarefasConcluidas { get; set; }
  public int TarefasEmAndamento { get; set; }
  public int TarefasAFazer { get; set; }



  // campos para mostrar na semana
  public int TarefasConcluidasEstaSemana { get; set; }
  public int TarefasEmAndamentoEstaSemana { get; set; }
  public int TarefasAFazerEstaSemana { get; set; }
  public int TotalEstaSemana { get; set; }
}