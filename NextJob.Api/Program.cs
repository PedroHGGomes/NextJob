using System;
using Asp.Versioning;
using Microsoft.EntityFrameworkCore;
using NextJob.Api.Data;
using NextJob.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Log para conferir o que esta rodando
Console.WriteLine($"Ambiente Atual: {builder.Environment.EnvironmentName}");

if (!builder.Environment.IsEnvironment("Testing"))
{
    Console.WriteLine("Modo Normal");

    // EF Core + Oracle em ambiente normal 
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseOracle(builder.Configuration.GetConnectionString("ConexaoOracle"))
    );

    builder.Services.AddHealthChecks()
        .AddDbContextCheck<AppDbContext>("Database");
}
else
{
    Console.WriteLine("Modo de Teste");

    // Em Testing
    builder.Services.AddHealthChecks();
}


builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Versionamento da API
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

// ML.NET
builder.Services.AddSingleton<MatchMlService>();

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .AllowAnyOrigin());
});

var app = builder.Build();

// Tracing simples
app.Use(async (context, next) =>
{
    var traceId = Guid.NewGuid().ToString();
    context.Response.Headers.Append("X-Trace-Id", traceId);
    await next();
});

// Swagger
app.UseSwagger();
app.UseSwaggerUI();


app.UseCors("AllowAll");

app.MapHealthChecks("/health");
app.MapControllers();

app.Run();

// Necessário para WebApplicationFactory<Program>
public partial class Program { }
