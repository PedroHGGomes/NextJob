using System;
using Asp.Versioning;
using Microsoft.EntityFrameworkCore;
using NextJob.Api.Data;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using NextJob.Api.Services;



var builder = WebApplication.CreateBuilder(args);

// Add Controllers
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

// EF Core + Oracle
if (!builder.Environment.IsEnvironment("Testing"))
{
    
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseOracle(builder.Configuration.GetConnectionString("ConexaoOracle"))
    );

    builder.Services.AddHealthChecks()
        .AddDbContextCheck<AppDbContext>("Database");
}
else
{
    
    builder.Services.AddHealthChecks();
}


//ML.NET
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
if (app.Environment.IsEnvironment("Testing"))
{
    // Apenas JSON no ambiente de teste
    app.UseSwagger();
}
else
{
    // Swagger completo no dev/prod
    app.UseSwagger();
    app.UseSwaggerUI();
}


if (!app.Environment.IsEnvironment("Testing"))
{
    app.UseHttpsRedirection();
}


app.UseCors("AllowAll");

app.MapHealthChecks("/health");

app.MapControllers();

app.Run();


public partial class Program { }
