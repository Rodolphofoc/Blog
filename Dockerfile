# Imagem base para execução do ASP.NET
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Imagem base para build (SDK)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar a solução e todos os projetos .csproj para o contêiner
COPY ["TaskManager.sln", "./"]
COPY ["Application/Application/Application.csproj", "Application/"]
COPY ["Apresentation/Api/Api.csproj", "Api/"]
COPY ["CrossCuting/CrossCuting.csproj", "CrossCuting/"]
COPY ["Domain/Domain/Domain.csproj", "Domain/"]
COPY ["Infrastructure/Infrastructure/Infrastructure.csproj", "Infrastructure/"]

# Restaurar as dependências
RUN dotnet restore "Blog.sln"

# Copiar o restante dos arquivos do projeto para o contêiner
COPY . .

# Construir a solução
RUN dotnet build "Blog.sln" -c Release -o /app/build

# Publicar o projeto
FROM build AS publish
RUN dotnet publish "Blog.sln" -c Release -o /app/publish

# Imagem final para execução do ASP.NET
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Definir o ponto de entrada do contêiner
ENTRYPOINT ["dotnet", "Blog.dll"]
