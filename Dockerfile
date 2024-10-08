# Etapa 1: Build da aplicação
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copia os arquivos de projeto e restaura as dependências
COPY *.csproj ./
RUN dotnet restore

# Copia o restante dos arquivos e faz o build
COPY . ./
RUN dotnet publish -c Release -o out

# Etapa 2: Configuração do runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .

# Define o comando que rodará sua aplicação
ENTRYPOINT ["dotnet", "BitBeakAPI.dll"]
