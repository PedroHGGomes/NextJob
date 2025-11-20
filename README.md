# NextJob
Projeto realizado para a entrega de C# - Global Solution

# NextJob.Api 

API RESTful em .NET 9 para gerenciamento de candidatos, vagas e cálculo de compatibilidade usando **ML.NET**, com foco em boas práticas REST, observabilidade e integrações modernas (Swagger, Health Checks, Versionamento, Oracle, etc.).

> Projeto desenvolvido para o Challenge / Sprint de .NET, com requisitos de:
> - Health Checks
> - Versionamento de API
> - Integração com banco Oracle via EF Core
> - Uso de ML.NET em um endpoint
> - Documentação via Swagger
> - (Opcional / Próximo passo) Segurança com API Key ou JWT  
> - (Opcional / Próximo passo) Testes com xUnit + WebApplicationFactory

---

## Tecnologias Utilizadas

- **.NET 9** (ASP.NET Core Web API)
- **C#**
- **Entity Framework Core 9** + **Oracle.EntityFrameworkCore**
- **ML.NET 5.0** (regressão para previsão de compatibilidade)
- **Swashbuckle.AspNetCore 10** (Swagger / OpenAPI)
- **Asp.Versioning.Http** (versionamento de API)
- **Health Checks** (`Microsoft.Extensions.Diagnostics.HealthChecks`)
- **CORS** liberado para chamadas externas
- **Logging** com console
- **Trace ID** customizado por requisição (header `X-Trace-Id`)

---

## Estrutura Geral do Projeto

```text
NextJob.Api/
├── Controllers/
│   └── v1/
│       └── MatchController.cs
├── Data/
│   └── AppDbContext.cs
├── Model/
│   ├── Candidate.cs
│   ├── JobOpening.cs
│   ├── MatchResult.cs
│   └── Requests/
│       └── MatchRequest.cs
├── ML/
│   └── MatchModelInput.cs
├── Services/
│   └── MatchMlService.cs
├── Properties/
│   └── launchSettings.json
├── Program.cs
└── NextJob.Api.csproj
NextJob.Test/
├── BasicIntegrationTests.cs
├── MatchMlService.cs
├── TestApplicationFactory.cs
└── NextJob.Test.csproj

```

**⚙️ Configuração de Ambiente**<br>

Connection String Oracle<br>

No appsettings.json (ou appsettings.Development.json), configure a connection string:<br>

{<br>
  "ConnectionStrings": {<br>
    "ConexaoOracle": "User Id=SEU_USUARIO;Password=SUA_SENHA;Data Source=SEU_HOST:1521/SEU_SERVICO"
  }<br>
}<br>

O Program.cs usa essa connection string:<br>

builder.Services.AddDbContext<AppDbContext>(options =><br>
    options.UseOracle(builder.Configuration.GetConnectionString("ConexaoOracle"))<br>
);

___________________________________________________________
**Ambiente (Development)**<br>

No Properties/launchSettings.json, o ambiente padrão deve ser Development para habilitar o Swagger:<br>

"environmentVariables": {<br>
  "ASPNETCORE_ENVIRONMENT": "Development"<br>
}<br>
___________________________________________________________
** Como Executar o Projeto**<br>

Na pasta do projeto NextJob.Api:

dotnet restore<br>
dotnet build<br>
dotnet run

Por padrão, a API sobe em uma porta configurada pelo Kestrel / launchSettings (por exemplo, http://localhost:5000).
___________________________________________________________
**Documentação via Swagger**

Quando a API está rodando em Development, o Swagger fica disponível em:

http://localhost:PORTA/swagger

O Swagger é configurado em Program.cs:<br>

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
___________________________________________________________
**Versionamento da API**

O projeto utiliza Asp.Versioning para versionamento:

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);<br>
    options.AssumeDefaultVersionWhenUnspecified = true;<br>
    options.ReportApiVersions = true;
});
Os controllers seguem o padrão:

namespace NextJob.Api.Controllers.v1

{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class MatchController : ControllerBase
    {
        // ...
    }
}
Exemplo de endpoint versionado:

POST /api/v1/Match

GET /api/v1/Match/{id}
___________________________________________________________
** Health Checks**<br>

Health check básico para verificar se o banco Oracle está acessível:

Configuração em Program.cs:

builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>("Database");
Mapeamento do endpoint:

app.MapHealthChecks("/health");

Testando Com a API rodando:<br>

GET http://localhost:PORTA/health
<br>
200 OK → aplicação e banco estão OK

Outro status → problema na conexão ou na aplicação
___________________________________________________________
**Endpoint com ML.NET (Match de Candidato x Vaga)**
<br>
O projeto contém um serviço de ML.NET que prevê a compatibilidade entre um candidato e uma vaga usando:

Score de habilidades obrigatórias (RequiredSkillsScore)

Score de habilidades desejáveis (DesiredSkillsScore)

Score de soft skills (SoftSkillsScore)

Anos de experiência do candidato (YearsOfExperience)
___________________________________________________________
** Serviço de ML: MatchMlService**
<br>
Arquivo: Services/MatchMlService.cs

Principais pontos:

Cria um MLContext

Monta um pequeno dataset de treino em memória (List<MatchTrainingRow>)

Usa um pipeline de regressão com:

_mlContext.Transforms.Concatenate(
        "Features",
        nameof(MatchModelInput.RequiredSkillsScore),
        nameof(MatchModelInput.DesiredSkillsScore),
        nameof(MatchModelInput.SoftSkillsScore),
        nameof(MatchModelInput.YearsOfExperience))
    .Append(_mlContext.Regression.Trainers.Sdca(
        labelColumnName: "Label",
        featureColumnName: "Features"));
Treina o modelo (pipeline.Fit(dataView))

Cria um PredictionEngine<MatchModelInput, MatchModelOutput>

Exposição de método público:

public float PredictCompatibility(
    double requiredScore,
    double desiredScore,
    double softScore,
    int yearsOfExperience)
Esse método retorna um valor entre 0 e 100 representando a compatibilidade prevista.
___________________________________________________________
** Modelo de Entrada/Saída de ML**
<br>
Arquivo: ML/MatchModelInput.cs

public class MatchModelInput
<br>
{<br>
    public float RequiredSkillsScore { get; set; }<br>
    public float DesiredSkillsScore { get; set; }}<br>
    public float SoftSkillsScore { get; set; }}<br>
    public float YearsOfExperience { get; set; }}<br>
}<br>
}

public class MatchModelOutput
{
    [ColumnName("Score")]}<br>
    public float Score { get; set; }}<br>
}<br>
}

**🔗 Registro do Serviço no Program.cs**

builder.Services.AddSingleton<MatchMlService>();
___________________________________________________________
<br>
**Endpoint de Cálculo de Compatibilidade (MatchController)**
<br>
Arquivo: Controllers/v1/MatchController.cs

Rota: POST /api/v1/Match

Fluxo principal:

- Recebe um MatchRequest com IDs de candidato e vaga.

- Busca Candidate e JobOpening no banco via AppDbContext.

- Calcula os scores textuais com uma função CalcScore, comparando textos de skills.

- Chama o ML.NET para prever o score final.

- Persiste um MatchResult com:

- RequiredSkillsScore

- DesiredSkillsScore

- SoftSkillsScore

- TotalCompatibility (resultado do ML.NET)

- Recomendações de currículo, skills, cursos e plano de carreira.

- Retorna 201 Created com links HATEOAS.

___________________________________________________________
Exemplo simplificado do uso do ML.NET dentro do controller:

var requiredScore = CalcScore(candidate.TechnicalSkills, job.RequiredSkills);
var desiredScore  = CalcScore(candidate.TechnicalSkills, job.DesiredSkills);
var softScore     = CalcScore(candidate.SoftSkills, job.SoftSkills);

var total = _matchMlService.PredictCompatibility(
    requiredScore,
    desiredScore,
    softScore,
    candidate.YearsOfExperience
);
___________________________________________________________
** CORS**
<br>

Para permitir que front-ends consumam a API (ex: React, Angular), foi configurado CORS liberando tudo:

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .AllowAnyOrigin());
});

app.UseCors("AllowAll");

___________________________________________________________
**Observabilidade: Logging e Trace ID**
<br>

Logging configurado para console:

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
Middleware simples para adicionar um X-Trace-Id em todas as respostas:

app.Use(async (context, next) =>
{
    var traceId = Guid.NewGuid().ToString();
    context.Response.Headers.Append("X-Trace-Id", traceId);
    await next();
});
<br>
Isso ajuda a rastrear requisições individualmente em logs.
___________________________________________________________
**Deploy da API**
<br>
A API está publicada na plataforma Render, em ambiente de produção.
<br>

**URL base**<br>

https://nextjob-1-qwwl.onrender.com
<br>

**Swagger**<br>

https://nextjob-1-qwwl.onrender.com/swagger
<br>

### Banco de Dados<br>

- O banco Oracle utilizado por esta API roda localmente e **não é acessível externamente**.<br>

- Por esse motivo, no ambiente online apenas endpoints que não dependem da base de dados funcionam.<br>
<br>

**Ambiente**<br>

ASPNETCORE_ENVIRONMENT = Production<br>

<br>
__________________________________________________________________________
**Como testar**
<br>
- Acesse `/swagger` para visualizar a documentação completa.<br>


Testes unitários (ML.NET)
<br>
Arquivo: NextJob.Tests/MatchMlServiceTests.cs
<br>
Garante que candidatos mais fortes têm score maior
<br>
Garante que o score sempre fica entre 0 e 100
<br>
Garante que o modelo é determinístico (mesma entrada → mesmo resultado)
<br>
Testes de integração (WebApplicationFactory)
<br>
Arquivo: NextJob.Tests/BasicIntegrationTests.cs
<br>
Usam WebApplicationFactory<Program> + TestApplicationFactory para:
<br>
Subir a API em memória usando banco InMemory
<br>
Verificar que o endpoint de Swagger existe:
<br>
GET /swagger/v1/swagger.json retorna 200
<br>
Verificar que o endpoint de HealthCheck existe:
<br>
GET /health não retorna 404
<br>
- Realize requisições HTTP diretamente pelos endpoints documentados.<br>

