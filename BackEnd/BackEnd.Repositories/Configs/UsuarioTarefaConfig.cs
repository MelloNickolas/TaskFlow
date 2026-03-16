using BackEnd.Dominio.Entidades;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories.Configs;

public class UsuarioTarefaConfig : IEntityTypeConfiguration<UsuarioTarefa>
{
  public void Configure(EntityTypeBuilder<UsuarioTarefa> builder)
  {
    builder.ToTable("UsuariosTarefas").HasKey(usuarioTarefa => usuarioTarefa.Id);

    builder.Property(nameof(UsuarioTarefa.DataAtribuicao)).HasDefaultValue(DateOnly.FromDateTime(DateTime.Now)).IsRequired();


    /*adicionamos os builders com os relacionamento no UsuarioTarefa pois ele é quem possui
    as chaves estrangeiras e onde ele guarda todos os relacionamentos*/
    
    builder
      .HasOne(UsuarioTarefa => UsuarioTarefa.Usuario)
      .WithMany(usuario => usuario.UsuarioTarefas)
      .HasForeignKey(usuarioTarefa => usuarioTarefa.UsuarioId);

    builder
        .HasOne(UsuarioTarefa => UsuarioTarefa.Tarefa)
        .WithMany(tarefa => tarefa.UsuariosTarefa)
        .HasForeignKey(usuarioTarefa => usuarioTarefa.TarefaId);
  }
}