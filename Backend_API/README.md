# Backend API - Escola High Tech

API REST em ASP.NET Core 10 para gerenciamento escolar com autenticacao JWT, autorizacao por perfis e CRUD unico de usuarios.

## Tecnologias

- ASP.NET Core 10
- Entity Framework Core
- SQL Server e SQLite em desenvolvimento/testes
- JWT Bearer
- FluentValidation
- Swagger/OpenAPI
- xUnit, Moq e FluentValidation.TestHelper
- Logging diario em arquivo

## Como Rodar Localmente

```bash
dotnet restore form_API.csproj
dotnet run
```

Em desenvolvimento, `appsettings.Development.json` usa SQLite local em `form-dev.db`.
Se `Jwt__Key` nao estiver definida em Development, a API cria uma chave local em `.local/jwt.key`, pasta ignorada pelo Git.
Em container ou producao, defina `Jwt__Key` e a connection string por variaveis de ambiente ou secrets do provedor.

## Docker Compose

Na raiz do repositorio:

```bash
cp .env.example .env
docker compose -f docker/docker-compose.yml up --build
```

Preencha o `.env` local com `MSSQL_SA_PASSWORD` e `JWT_KEY` antes de subir os containers.
Esse arquivo local e ignorado pelo Git; mantenha apenas `.env.example` versionado.

Acessos padrao:

- API: `http://localhost:5000`
- Swagger: `http://localhost:5000/swagger`
- Frontend: `http://localhost:8080`
- SQL Server: `localhost,1433`

## Usuario Inicial

```text
Email: admin@escola.com
Senha: Senha@123
Perfil: Administrador
```

Usuarios criados pelo cadastro recebem a senha inicial:

```text
Senha@252525
```

## Logs

Os logs ficam na raiz do projeto backend:

```text
Backend_API/logs/backend-api-YYYYMMDD.log
```

O provider `DailyFileLoggerProvider` registra:

- Startup da API.
- Preparacao do banco e migrations.
- Inicio e fim das requisicoes HTTP.
- Metodo, rota, usuario, status e tempo de execucao.
- Erros capturados pelos controllers.
- Tentativas de login recusadas, login bem-sucedido e troca de senha.

No Docker Compose, o volume `../Backend_API/logs:/app/logs` mantem os logs do container nessa mesma pasta.

## Arquitetura

O backend usa arquitetura em camadas:

- `Controllers`: recebem requisicoes HTTP, aplicam autorizacao e retornam status codes.
- `Validators`: validam ViewModels via FluentValidation.
- `Services`: executam regras de negocio e orquestram persistencia.
- `Data`: contem `DataContext`, migrations e repository.
- `Models`: representam entidades persistidas.
- `ViewModels`: representam contratos de entrada e saida da API.
- `Security`: hash e politica de senha.
- `Swagger`: filtros OpenAPI para documentar endpoints protegidos.
- `Logging`: provider de log diario em arquivo.

## Endpoints Principais

| Entidade | Rotas |
| --- | --- |
| Auth | `POST /api/Auth/login`, `GET /api/Auth/me`, `POST /api/Auth/alterar-senha` |
| Usuarios | `GET /api/usuarios`, `GET /api/usuarios/{id}`, `GET /api/usuarios/perfis`, `POST /api/usuarios`, `PUT /api/usuarios/{id}`, `DELETE /api/usuarios/{id}` |

## Autorizacao

- `Administrador`: acesso completo.
- `Professor`: pode cadastrar usuarios apenas com perfil `Aluno` e consultar usuarios.
- `Aluno`: nao cadastra usuarios; pode corrigir apenas seus proprios dados de nome, email e telefone.

## Testes

```bash
dotnet test form_API.Tests/form_API.Tests.csproj
```

## Documentacao

- Markdown tecnico: `../docs/backend-tecnico.md`
- PDF tecnico: `../docs/backend-tecnico.pdf`
- PDF tecnico completo do backend: `docs/documentacao-tecnica-backend.pdf`
- HTML fonte do PDF completo: `docs/documentacao-tecnica-backend.html`
- Swagger: `/swagger` em ambiente de desenvolvimento

No Swagger, execute `POST /api/Auth/login`, copie o token e use `Authorize` com:

```text
Bearer {token}
```
