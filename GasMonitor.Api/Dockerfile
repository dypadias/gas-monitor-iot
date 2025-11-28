# Estágio 1: Construção (Build)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia o arquivo de projeto e baixa as dependências
COPY ["GasMonitor.Api.csproj", "./"]
RUN dotnet restore "GasMonitor.Api.csproj"

# Copia todo o resto do código da API e compila
COPY . .
RUN dotnet publish "GasMonitor.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Estágio 2: Execução (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Configuração Especial para o Render (Porta 8080)
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

# Prepara a aplicação para rodar
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "GasMonitor.Api.dll"]