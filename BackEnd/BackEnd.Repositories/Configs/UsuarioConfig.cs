using BackEnd.Dominio.Entidades;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories.Configs;

public class UsuarioConfig : IEntityTypeConfiguration<Usuario>
{
  public void Configure(EntityTypeBuilder<Usuario> builder)
  {
    builder.ToTable("Usuarios").HasKey(usuario => usuario.Id);

    // Gera o ID automaticamente na sua criação
    builder.Property(nameof(Usuario.Id)).HasColumnName("Id").ValueGeneratedOnAdd();
    builder.Property(nameof(Usuario.Nome)).HasColumnName("Nome").HasMaxLength(50).IsRequired();
    builder.Property(nameof(Usuario.Email)).HasColumnName("Email").HasMaxLength(50).IsRequired();
    builder.HasIndex(usuario => usuario.Email).IsUnique(); //permite somente 1 email por cadastro
    builder.Property(nameof(Usuario.SenhaHash)).HasColumnName("SenhaHash").HasMaxLength(255).IsRequired();
    builder.Property(nameof(Usuario.Ativo)).HasColumnName("Ativo");
    builder.Property(nameof(Usuario.Funcao)).HasColumnName("Funcao").IsRequired();
  }
}