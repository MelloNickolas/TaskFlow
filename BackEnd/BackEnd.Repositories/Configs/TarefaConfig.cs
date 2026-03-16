using BackEnd.Dominio.Entidades;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories.Configs;

public class TarefaConfig : IEntityTypeConfiguration<Tarefa>
{
  public void Configure(EntityTypeBuilder<Tarefa> builder)
  {
    builder.ToTable("Tarefas").HasKey(tarefa => tarefa.Id);

    // Gera o ID automaticamente na sua criação
    builder.Property(nameof(Tarefa.Id)).HasColumnName("Id").ValueGeneratedOnAdd();

    builder.Property(nameof(Tarefa.Titulo)).HasColumnName("Titulo").HasMaxLength(100).IsRequired();
    builder.Property(nameof(Tarefa.Descricao)).HasColumnName("Descricao").HasMaxLength(500);

    // Aqui estamos garantindo que tem que passar um tipo data para o nosso banco de dados, porem fique esperto
    // alguns bancos isso nao tera efeito!
    builder.Property(nameof(Tarefa.DataCriada)).HasColumnName("DataCriada").HasColumnType("date").IsRequired();
    builder.Property(nameof(Tarefa.Prazo)).HasColumnName("Prazo").HasColumnType("date").IsRequired();

    builder.Property(nameof(Tarefa.Prioridade)).HasColumnName("Prioridade").IsRequired();
    builder.Property(nameof(Tarefa.Status)).HasColumnName("Status").IsRequired();
  }
}