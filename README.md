# NextJob
Projeto realizado para a entrega de C# - Global Solution

# NextJob.Api 🧠💼

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

## 🏗️ Tecnologias Utilizadas

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

## 📁 Estrutura Geral do Projeto

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
Obs.: alguns nomes podem variar levemente dependendo da sua modelagem, mas essa é a ideia geral.

⚙️ Configuração de Ambiente
🔑 Connection String Oracle
No appsettings.json (ou appsettings.Development.json), configure a connection string:

json
Copiar código
{
  "ConnectionStrings": {
    "ConexaoOracle": "User Id=SEU_USUARIO;Password=SUA_SENHA;Data Source=SEU_HOST:1521/SEU_SERVICO"
  }
}
O Program.cs usa essa connection string:

csharp
Copiar código
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("ConexaoOracle"))
);
🌍 Ambiente (Development)
No Properties/launchSettings.json, o ambiente padrão deve ser Development para habilitar o Swagger:

json
Copiar código
"environmentVariables": {
  "ASPNETCORE_ENVIRONMENT": "Development"
}
🚀 Como Executar o Projeto
Na pasta do projeto NextJob.Api:

bash
Copiar código
dotnet restore
dotnet build
dotnet run
Por padrão, a API sobe em uma porta configurada pelo Kestrel / launchSettings (por exemplo, http://localhost:5000).

📚 Documentação via Swagger
Quando a API está rodando em Development, o Swagger fica disponível em:

text
Copiar código
http://localhost:PORTA/swagger
O Swagger é configurado em Program.cs:

csharp
Copiar código
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
🧬 Versionamento da API
O projeto utiliza Asp.Versioning para versionamento:

csharp
Copiar código
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});
Os controllers seguem o padrão:

csharp
Copiar código
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

❤️ Health Checks
Health check básico para verificar se o banco Oracle está acessível:

Configuração em Program.cs
csharp
Copiar código
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>("Database");
Mapeamento do endpoint:

csharp
Copiar código
app.MapHealthChecks("/health");
Testando
Com a API rodando:

text
Copiar código
GET http://localhost:PORTA/health
200 OK → aplicação e banco estão OK

Outro status → problema na conexão ou na aplicação

🧠 Endpoint com ML.NET (Match de Candidato x Vaga)
O projeto contém um serviço de ML.NET que prevê a compatibilidade entre um candidato e uma vaga usando:

Score de habilidades obrigatórias (RequiredSkillsScore)

Score de habilidades desejáveis (DesiredSkillsScore)

Score de soft skills (SoftSkillsScore)

Anos de experiência do candidato (YearsOfExperience)

🧩 Serviço de ML: MatchMlService
Arquivo: Services/MatchMlService.cs

Principais pontos:

Cria um MLContext

Monta um pequeno dataset de treino em memória (List<MatchTrainingRow>)

Usa um pipeline de regressão com:

csharp
Copiar código
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

csharp
Copiar código
public float PredictCompatibility(
    double requiredScore,
    double desiredScore,
    double softScore,
    int yearsOfExperience)
Esse método retorna um valor entre 0 e 100 representando a compatibilidade prevista.

🧠 Modelo de Entrada/Saída de ML
Arquivo: ML/MatchModelInput.cs

csharp
Copiar código
public class MatchModelInput
{
    public float RequiredSkillsScore { get; set; }
    public float DesiredSkillsScore { get; set; }
    public float SoftSkillsScore { get; set; }
    public float YearsOfExperience { get; set; }
}

public class MatchModelOutput
{
    [ColumnName("Score")]
    public float Score { get; set; }
}
🔗 Registro do Serviço no Program.cs
csharp
Copiar código
builder.Services.AddSingleton<MatchMlService>();
🎯 Endpoint de Cálculo de Compatibilidade (MatchController)
Arquivo: Controllers/v1/MatchController.cs
Rota: POST /api/v1/Match

Fluxo principal:

Recebe um MatchRequest com IDs de candidato e vaga.

Busca Candidate e JobOpening no banco via AppDbContext.

Calcula os scores textuais com uma função CalcScore, comparando textos de skills.

Chama o ML.NET para prever o score final.

Persiste um MatchResult com:

RequiredSkillsScore

DesiredSkillsScore

SoftSkillsScore

TotalCompatibility (resultado do ML.NET)

Recomendações de currículo, skills, cursos e plano de carreira.

Retorna 201 Created com links HATEOAS.

Exemplo simplificado do uso do ML.NET dentro do controller:

csharp
Copiar código
var requiredScore = CalcScore(candidate.TechnicalSkills, job.RequiredSkills);
var desiredScore  = CalcScore(candidate.TechnicalSkills, job.DesiredSkills);
var softScore     = CalcScore(candidate.SoftSkills, job.SoftSkills);

var total = _matchMlService.PredictCompatibility(
    requiredScore,
    desiredScore,
    softScore,
    candidate.YearsOfExperience
);
🌐 CORS
Para permitir que front-ends consumam a API (ex: React, Angular), foi configurado CORS liberando tudo:

csharp
Copiar código
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .AllowAnyOrigin());
});

app.UseCors("AllowAll");
🔍 Observabilidade: Logging e Trace ID
Logging configurado para console:

csharp
Copiar código
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
Middleware simples para adicionar um X-Trace-Id em todas as respostas:

csharp
Copiar código
app.Use(async (context, next) =>
{
    var traceId = Guid.NewGuid().ToString();
    context.Response.Headers.Append("X-Trace-Id", traceId);
    await next();
});
Isso ajuda a rastrear requisições individualmente em logs.
