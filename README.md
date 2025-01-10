# Clientes POC

Este projeto é uma aplicação web que utiliza Blazor Server para a camada de apresentação e ASP.NET Core API para a camada de backend. O sistema gerencia clientes, incluindo o cadastro de dados pessoais e endereços, com a possibilidade de enviar e armazenar logotipos. A arquitetura do sistema segue o padrão de **Arquitetura em Camadas**, com uma comunicação robusta entre o frontend e o backend.

## Tecnologias Usadas

- **Blazor Server**: Camada de apresentação (UI) baseada em Blazor.
- **ASP.NET Core**: Camada de backend com uma API RESTful para CRUD de clientes e endereços.
- **Entity Framework Core**: Para persistência de dados no banco de dados.
- **SQL Server**: Banco de dados utilizado para armazenar clientes, endereços e logotipos.
- **ASP.NET Core MVC**: Para renderização de páginas dinâmicas no frontend.
- **AutoMapper**: Para mapear entre os modelos de entidade e os modelos de visualização (DTOs).
- **Memory Cache**: Para otimizar o desempenho ao armazenar em cache dados frequentemente acessados.

---

## Estrutura do Projeto

### Camadas do Projeto

- **Camada de Apresentação (UI)**: Utiliza **Blazor Server** e **Razor Pages** para exibir e interagir com os dados.
- **Camada de Aplicação**: Contém a lógica de negócios e os casos de uso para gerenciar as operações de cliente e endereços.
- **Camada de Domínio**: Define as entidades principais, como `Customer`, `Address` e `Logo`, além de seus DTOs e validações.
- **Camada de Infraestrutura**: Gerencia o acesso ao banco de dados com **Entity Framework Core** e implementa o cache em memória.
- **Serviços Externos**: Responsáveis pela comunicação com APIs externas (se necessário) e serviços de armazenamento de arquivos.

---

## Banco de Dados

O banco de dados é composto por duas principais tabelas:

### **Tabela de Clientes (Customers)**

| Coluna             | Tipo                |
|--------------------|---------------------|
| `Id`               | `UNIQUEIDENTIFIER`  |
| `Name`             | `NVARCHAR(100)`     |
| `Email`            | `NVARCHAR(100)`     |
| `Logo`             | `VARBINARY(MAX)`    |
| `LogoContentType`  | `NVARCHAR(MAX)`     |
| `LogoFileName`     | `NVARCHAR(MAX)`     |

### **Tabela de Endereços (Addresses)**

| Coluna  | Tipo              |
|---------|-------------------|
| `Id`    | `UNIQUEIDENTIFIER` |
| `Street`| `NVARCHAR(200)`    |
| `City`  | `NVARCHAR(100)`    |
| `State` | `NVARCHAR(100)`    |
| `CustomerId` | `UNIQUEIDENTIFIER` (Chave estrangeira para a tabela `Customers`) |

---

## Tipos SQL

### **Tipo de Tabela: `AddressTableType`**

```sql
CREATE TYPE AddressTableType AS TABLE
(
    Street NVARCHAR(200),
    City NVARCHAR(100),
    State NVARCHAR(100),
    CustomerId UNIQUEIDENTIFIER
);
```

Procedimentos Armazenados

1. Criar Cliente (AddCustomer)
   
```sql
CREATE PROCEDURE AddCustomer
    @Id UNIQUEIDENTIFIER,
    @Name NVARCHAR(100),
    @Email NVARCHAR(100),
    @Logo varbinary(max),
    @LogoContentType NVARCHAR(max),
    @LogoFileName NVARCHAR(max),
    @Addresses AddressTableType READONLY
AS
BEGIN
    BEGIN TRANSACTION;

    -- Inserir Cliente
    INSERT INTO Customers (Id, Name, Email, Logo, LogoContentType, LogoFileName)
    VALUES (@Id, @Name, @Email, @Logo, @LogoContentType, @LogoFileName);

    -- Inserir Endereços
    INSERT INTO Addresses (Id, Street, City, State, CustomerId)
    SELECT NEWID(), Street, City, State, @Id FROM @Addresses;

    COMMIT TRANSACTION;
END;
```

3. Atualizar Cliente (UpdateCustomer)
   
```sql
CREATE PROCEDURE UpdateCustomer
    @Id UNIQUEIDENTIFIER,
    @Name NVARCHAR(100),
    @Email NVARCHAR(100),
    @Logo varbinary(max),
    @LogoContentType NVARCHAR(max),
    @LogoFileName NVARCHAR(max),
    @Addresses AddressTableType READONLY
AS
BEGIN
    BEGIN TRANSACTION;

    -- Atualizar Cliente
    UPDATE Customers
    SET Name = @Name,
        Email = @Email,
        Logo = @Logo,
        LogoContentType = @LogoContentType,
        LogoFileName = @LogoFileName
    WHERE Id = @Id;

    -- Atualizar Endereços existentes
    UPDATE a
    SET Street = temp.Street,
        City = temp.City,
        State = temp.State
    FROM Addresses a
    INNER JOIN @Addresses temp
        ON a.Street = temp.Street
        AND a.City = temp.City
        AND a.State = temp.State
        AND a.CustomerId = @Id;

    -- Remover Endereços antigos não presentes em @Addresses
    DELETE FROM Addresses
    WHERE CustomerId = @Id AND NOT EXISTS (
        SELECT 1 FROM @Addresses a
        WHERE a.Street = Addresses.Street
          AND a.City = Addresses.City
          AND a.State = Addresses.State
    );

    -- Inserir novos Endereços
    INSERT INTO Addresses (Id, Street, City, State, CustomerId)
    SELECT NEWID(), Street, City, State, @Id
    FROM @Addresses a
    WHERE NOT EXISTS (
        SELECT 1 FROM Addresses
        WHERE Addresses.Street = a.Street
          AND Addresses.City = a.City
          AND Addresses.State = a.State
          AND Addresses.CustomerId = @Id
    );

    COMMIT TRANSACTION;
END;
```

Comandos de Migrations
Para configurar o Entity Framework Core e gerar a base de dados, siga os passos abaixo.

Instalar EF 6
Instale a ferramenta dotnet-ef para gerenciamento de migrations:

```bash
dotnet tool install --global dotnet-ef --version 6.*
```

Rodar a Migration Inicial

Crie e aplique a migration inicial para configurar o banco de dados:

```bash
dotnet ef migrations add Initial --startup-project ClientesPOCApi --project Infrastructure
```

Como Rodar o Projeto
Clone o repositório para sua máquina local.

Certifique-se de que todas as dependências do projeto estão instaladas.

Execute o comando para rodar a aplicação:

```bash
dotnet run --project ClientesPOCApi
```

O projeto estará disponível em http://localhost:5000.

Considerações Finais
Esse projeto segue as melhores práticas de arquitetura para aplicações .NET, com uma separação clara de responsabilidades,
validação de dados e controle de fluxo. A comunicação entre o frontend Blazor e o backend API RESTful é eficiente, e a
utilização de procedimentos armazenados permite um controle de dados robusto e otimizado.

Autor: Marlon Almeida
