# BitBeak - Backend

## Introdução
O BitBeak é uma plataforma educacional inovadora que incorpora gamificação e Inteligência Artificial Generativa para apoiar o ensino de Programção em Engenharia de Software. Este repositório abriga o backend do sistema, que é implementado em C# utilizando .NET e Entity Framework para a gestão de dados no SQL Server.

## Pré-requisitos
Para rodar o backend do BitBeak em sua máquina, você precisará dos seguintes componentes instalados:
- .NET 8.0 SDK ou mais recente
- SQL Server 2019 ou mais recente
- Uma IDE compatível com C#, como o Visual Studio 2022

## Configuração  

### Passo a passo para configurar o ambiente de desenvolvimento:  

1. **Clonar o Repositório**  
  Clone o repositório em sua máquina local usando o seguinte comando:  
  ```bash git clone https://github.com/seu-usuario/bitbeak-backend.git```

2. **Instalar Dependências**  
  No terminal, navegue até a pasta do projeto clonado e execute:
  ```dotnet restore ```  
 
3. **Configurar Banco de Dados**  
  - Configure a string de conexão no arquivo appsettings.json:  
  ```
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=BitBeakDB;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
  ```  
  - Aplique as migrações para o banco de dados:
  ```dotnet ef database update```  

**Uso**  
  Para iniciar o servidor, execute:
  ```dotnet run```  
  A API estará disponível em http://localhost:????.  

**Contato**  
  Gustavo Henrique Alves - gustavo@email  
  Marcos Vinicius Alves de Souza - sbmarcos777@gmail.com  

**Agradecimentos**  
  - Professor Tiago Navarro por sua orientação e suporte.  
  - Todos os contribuidores e testadores que ajudaram a melhorar o projeto.  
