# Clínica Veterinária UVV

Projeto acadêmico da disciplina de Desenvolvimento Web Back-end.

A aplicação permite que tutores criem uma conta, façam login e realizem o gerenciamento de consultas veterinárias dos seus pets. O sistema foi desenvolvido com ASP.NET Core MVC, Entity Framework Core e SQL Server.

## Integrantes

- Pedro Henrique Lopes de Almeida

## Funcionalidades

- Cadastro de tutor usando requisição HTTP POST
- Login e logout com autenticação baseada em cookies
- Proteção das rotas de consultas com `[Authorize]`
- Cadastro, listagem, edição e exclusão de consultas veterinárias
- Consultas vinculadas ao tutor autenticado
- Especialidades disponíveis:
  - Gastroenterologia
  - Clínica Geral Felina
- Regra de negócio: Clínica Geral Felina é permitida somente para animais da espécie Gato
- Validações com Data Annotations
- Bloqueio de consultas em datas e horários passados
- Persistência com Entity Framework Core, SQL Server e Migrations no padrão Code First

## Tecnologias

- C#
- .NET 8
- ASP.NET Core MVC
- Entity Framework Core
- SQL Server
- Bootstrap

## Pré-requisitos

Antes de iniciar, instale:

- .NET SDK 8
- SQL Server, SQL Server Express ou Docker
- Docker, caso opte por executar o SQL Server em container

## Configuração

1. Clone o repositório:

```bash
git clone https://github.com/SEU_USUARIO/Clinica_veterinaria_UVV.git
cd ClinicaVeterinariaUVV
```

2. Restaure a ferramenta local do Entity Framework:

```bash
dotnet tool restore
```

3. Crie o arquivo `appsettings.Development.json` na raiz do projeto.

Exemplo para SQL Server em Docker:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=ClinicaVeterinariaUVV;User Id=sa;Password=SUA_SENHA_FORTE;TrustServerCertificate=True;"
  }
}
```

O arquivo `appsettings.Development.json` é ignorado pelo Git para impedir que senhas sejam enviadas ao repositório.

## Banco de dados com Docker

Para iniciar o SQL Server localmente via Docker:

```bash
docker run \
  --name sqlserver-clinica \
  -e "ACCEPT_EULA=Y" \
  -e "MSSQL_SA_PASSWORD=SUA_SENHA_FORTE" \
  -p 1433:1433 \
  -v sqlserver_clinica_data:/var/opt/mssql \
  -d mcr.microsoft.com/mssql/server:2022-latest
```

A senha deve conter letras maiúsculas, letras minúsculas, números e caracteres especiais.

Para verificar se o container está em execução:

```bash
docker ps
```

Para iniciar um container já criado:

```bash
docker start sqlserver-clinica
```

## Migrations e execução

Com o SQL Server em execução e a connection string configurada:

```bash
dotnet restore
dotnet tool restore
dotnet ef database update
dotnet run
```

Caso as migrations ainda não existam, crie-as antes:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

A aplicação exibirá no terminal uma URL semelhante a:

```text
Now listening on: http://localhost:5000
```

Abra essa URL em seu navegador.

## Roteiro de demonstração

1. Acessar a página inicial.
2. Criar uma conta de tutor.
3. Fazer login.
4. Agendar uma consulta de Gastroenterologia.
5. Agendar uma consulta de Clínica Geral Felina para um gato.
6. Tentar agendar Clínica Geral Felina para um cachorro e demonstrar a validação.
7. Tentar agendar uma consulta em data passada e demonstrar a validação.
8. Editar uma consulta.
9. Excluir uma consulta.
10. Fazer logout e tentar acessar a rota `/Consultas` sem estar autenticado.

## Vídeo demonstrativo
[Visualização do Sistema de Agendamento Veterinario](https://youtu.be/5OkxmruM5tM)

