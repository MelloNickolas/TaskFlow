using Microsoft.EntityFrameworkCore;
using BackEnd.Application;
using BackEnd.Repositories;
using BackEnd.Services;
using BackEnd.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using BCrypt.Net;

Console.WriteLine(BCrypt.Net.BCrypt.HashPassword("123"));

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
builder.Services.AddScoped<IRelatorioApplication, RelatorioApplication>();

/*
Registramos os repositories com AddScoped para registrar a implementação
com ciclo de vida por requisição, além de comunicar que quando alguem solicitar um IUsuarioRepository,
ele deve entregar um UsuarioRepository.
*/
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<ITarefaRepository, TarefaRepository>();
builder.Services.AddScoped<IUsuarioTarefaRepository, UsuarioTarefaRepository>();



// Registrando o serviço de geração de token JWT
builder.Services.AddScoped<IJwtService, JwtService>();



/*
adicionando o contexto que ja definimos antes,
ali vamos passar a string de conexao e as configurações do nosso database
ADICIONE A STRING NO APPSETINGS.JSON
*/
builder.Services.AddDbContext<BackEndContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));




// Swagger
builder.Services.AddEndpointsApiExplorer();

/*
O SwaggerGen é o gerador de documentação do Swagger — ele analisa seus controllers e endpoints e gera automaticamente a interface visual que você usa para testar a API.

O AddSwaggerGen configura esse gerador. Quando você adiciona o AddSecurityDefinition dentro dele, está dizendo ao Swagger — 
"essa API usa autenticação Bearer, mostre um botão para o usuário inserir o token antes de testar os endpoints".
*/
builder.Services.AddSwaggerGen(options =>
{
    /*
    aqui vamos definir a autorizaçao que vamos passa, vai ter um nome, vai ser do tipo HTTP, do scheme bearer que criamos uma 
    autenticaçao nesse modelo validando todos os dados, o formato que vai ser em JWT, e tudo isso vai estar no nosso header do esquema
    */
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT assim: Bearer {seu token}"
    });

    /*
    vamos adicionar uma requisiçao do tipo SecurityRequirement, e dentro dela vai ter um schema que vai ter uma refencia com um esquema de segurança do ID Bearer
    Em outras palavras — você está dizendo ao Swagger que todos os endpoints precisam do token Bearer por padrão.
    */

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});


// Configuração da autenticação JWT

/*
"as variáveis que criamos vão pegar os dados do appsettings.json para validar os tokens recebidos nas requisições"
*/
var jwtKey = builder.Configuration["Jwt:Key"]!;
var jwtIssuer = builder.Configuration["Jwt:Issuer"]!;
var jwtAudience = builder.Configuration["Jwt:Audience"]!;

/* 
aqui vamo validar cada informação que passou, e vamos verificar a autenticação, além de pegar a nossa chave de segurança

ValidateIssuer — verifica se o token foi emitido pela sua API
ValidateAudience — verifica se o token é destinado ao seu frontend
ValidateLifetime — verifica se o token não expirou
ValidateIssuerSigningKey — verifica se a assinatura do token é válida usando a chave secreta

E o AddAuthentication("Bearer") diz ao .NET que o esquema de autenticação padrão é o Bearer Token 
que é o formato Authorization: Bearer xxxxx.yyyyy.zzzzz que o frontend vai mandar no header.
*/
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey))
        };
    });



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

/* Antes de autorizamos vamos autenticar o nosso usuário */
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();



