using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BackEnd.Dominio.Entidades;
using BackEnd.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;


namespace BackEnd.Services;

public class JwtService : IJwtService
{

  /* 
  Injeçao de dependencia da interface do próprio .NET
  Ela permite ler as coonfigurações que definimos no APPSETTINGS.JSON

  Então a injeção de dependência aqui serve para acessar os valores que você configurou — Jwt:Key, Jwt:Issuer, Jwt:Audience.
  */
  private readonly IConfiguration _configuration;
  public JwtService(IConfiguration configuration)
  {
    _configuration = configuration;
  }

  public string GerarToken(Usuario usuario)
  {

    /*
    Encoding.UTF8.GetBytes — converte a string da chave "taskflow-chave-secreta..." em bytes, porque o algoritmo de criptografia trabalha com bytes, não com texto.

    SymmetricSecurityKey — cria uma chave simétrica, ou seja, a mesma chave é usada para assinar e validar o token. Só o seu backend conhece essa chave — 
    por isso ela fica no appsettings.json e nunca é exposta.

    O ! no final é o operador null-forgiving — diz ao compilador que você garante que o valor não é null.
    */
    var Key = new SymmetricSecurityKey(
      Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));



    /*
    Cria uma credencial de segurança usando a chave que temos no appsettings json e criptografando ela com o SecurityAlgorithms
    HmacSha256 é o algoritmo específico — é um dos mais usados para JWT por ser rápido e seguro.
    */
    var credentials = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256);



    /*
      Os Claims são as informações do usuário que ficam dentro do token.
      Quando o frontend mandar o token numa requisição, o backend consegue extrair esses Claims e saber:

      Quem é o usuário (id)
      Qual o  email dele (email)
      Qual a função dele (funcao) — Desenvolvedor, Gerente ou Administrador

      Isso é importante para as permissões — quando implementar, você vai verificar 
      o Claim funcao para saber se o usuário pode ou não executar determinada ação.
    */
    var claims = new[]
    {
      new Claim("id", usuario.Id.ToString()),
      new Claim("email", usuario.Email),

      /*
      tem um Claim especial chamado ClaimTypes.Role que é o que o [Authorize(Roles)] usa para verificar permissões. Você está usando "funcao" que é um nome customizado
      — o .NET não sabe que é a função do usuário.
      É como se você guardasse o cargo do funcionário numa gaveta chamada "funcao", mas o segurança da empresa só procura na gaveta chamada "role". Ele não vai achar nada!
      Então a solução é simples — troca o nome do Claim no JwtService de "funcao" para ClaimTypes.Role
      */
      new Claim(ClaimTypes.Role, usuario.Funcao.ToString())
    };


    /*
    esse é o nosso token, onde vamos passar o issuer no appsettings json junto com o audience

    o claim que criamos para validar tuod
    o expires que o tempo que esse token vai demorar para expirar
    e as credenciais para verficarmos se é realmente esse usuário que pode entrar com nossa senha.
    */
    var token = new JwtSecurityToken(
      issuer: _configuration["Jwt:Issuer"],
      audience: _configuration["Jwt:Audience"],
      claims: claims,
      expires: DateTime.UtcNow.AddHours(8),
      signingCredentials: credentials
    );


    // Ele serializa tudo isso em uma string no formato xxxxx.yyyyy.zzzzz que é o JWT final.
    return new JwtSecurityTokenHandler().WriteToken(token);
  }

  // agora vai para o PROGRAM.CS vamos autenticar tudo lá!
}