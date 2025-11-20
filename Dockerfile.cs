# =========================
# STAGE 1 - BUILD
# =========================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia apenas o csproj primeiro (melhor cache)
COPY NextJob.Api/NextJob.Api.csproj ./NextJob.Api/
RUN dotnet restore ./NextJob.Api/NextJob.Api.csproj

# Copia o restante do código
COPY . .

WORKDIR /src/NextJob.Api
RUN dotnet publish NextJob.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

# =========================
# STAGE 2 - RUNTIME
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

# Porta que o Render usa internamente
ENV ASPNETCORE_URLS=http://0.0.0.0:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "NextJob.Api.dll"]
# =========================
# STAGE 1 - BUILD
# =========================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia apenas o csproj primeiro (melhor cache)
COPY NextJob.Api/NextJob.Api.csproj ./NextJob.Api/
RUN dotnet restore ./NextJob.Api/NextJob.Api.csproj

# Copia o restante do código
COPY . .

WORKDIR /src/NextJob.Api
RUN dotnet publish NextJob.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

# =========================
# STAGE 2 - RUNTIME
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

# Porta que o Render usa internamente
ENV ASPNETCORE_URLS=http://0.0.0.0:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "NextJob.Api.dll"]
