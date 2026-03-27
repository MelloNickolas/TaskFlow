using BackEnd.Dominio.Entidades;
using BackEnd.Repositories.Configs;
using Microsoft.EntityFrameworkCore;


/* Exstamos herdando do contexto os meios para que nosso código se transforme em um banco de dados*/
public class BackEndContext : DbContext
{
  /* Estamos setando nossas entidades para tabelas no banco de dados */
  public DbSet<Usuario> Usuarios { get; set; }
  public DbSet<Tarefa> Tarefas { get; set; }
  public DbSet<UsuarioTarefa> UsuariosTarefas { get; set; }


  // Ele vai definir a string de conexao, as opçoes
  private readonly DbContextOptions _options;
  public BackEndContext() { }
  public BackEndContext(DbContextOptions options) : base(options)
  {
    _options = options;
  }


  // Ele vai definir o banco de dados que vamos usar,e qual o caminho do arquivo que vamos guardar
  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    if(_options == null)
    optionsBuilder.UseSqlite(@"Filename=./backendTaskFlow.sqlite;");
  }


  /* Indica as configurações de mapeamento para nosso banco de dados */
  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfiguration(new UsuarioConfig());
    modelBuilder.ApplyConfiguration(new TarefaConfig());
    modelBuilder.ApplyConfiguration(new UsuarioTarefaConfig());
  }
}