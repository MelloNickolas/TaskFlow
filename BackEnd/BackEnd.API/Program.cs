using Microsoft.EntityFrameworkCore;
using BackEnd.Application;
using BackEnd.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

/*
AddScoped registra a implementação com ciclo de vida por requisição — 
uma nova instância é criada a cada requisição HTTP e descartada ao final dela.

Lembre-se essa requisição vai passar para a aplicattion e ela vaii se comunicar e depois retornar.
*/
builder.Services.AddScoped<IUsuarioApplication, UsuarioApplication>();
builder.Services.AddScoped<ITarefaApplication, TarefaApplication>();
builder.Services.AddScoped<IUsuarioTarefaApplication, UsuarioTarefaApplication>();

/*
Registramos os repositories com AddScoped para registrar a implementação
com ciclo de vida por requisição, além de comunicar que quando alguem solicitar um IUsuarioRepository,
ele deve entregar um UsuarioRepository.
*/
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<ITarefaRepository, TarefaRepository>();
builder.Services.AddScoped<IUsuarioTarefaRepository, UsuarioTarefaRepository>();


/*
adicionando o contexto que ja definimos antes,
ali vamos passar a string de conexao e as configurações do nosso database
ADICIONE A STRING NO APPSETINGS.JSON
*/
builder.Services.AddDbContext<BackEndContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));




// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



/* o CORS controla quais origens podem fazer requisições para sua API.
Por exemplo, você configurou http://localhost:3000 — isso significa que só o frontend rodando nessa porta pode chamar sua API. 
Se tentar de outra origem, o navegador bloqueia.
*/
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("http://localhost:3000")
            .SetIsOriginAllowedToAllowWildcardSubdomains()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
