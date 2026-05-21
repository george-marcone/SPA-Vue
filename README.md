# SPA-Vue

Aplicacao escolar com backend ASP.NET Core e frontend Nuxt 3 em modo SPA.

## Documentacao

- Documentacao tecnica do backend: [docs/backend-tecnico.md](docs/backend-tecnico.md)
- Swagger/OpenAPI da API: `http://localhost:8080/swagger` quando a API roda em container, ou a URL exibida por `dotnet run`.

## Backend

O backend fica em `Backend_API` e usa ASP.NET Core 10, Entity Framework Core, SQL Server, JWT Bearer, FluentValidation e Swagger.

```bash
cd Backend_API
dotnet restore form_API.csproj
dotnet run
```

Comandos uteis:

```bash
dotnet build
dotnet test form_API.Tests/form_API.Tests.csproj
dotnet ef database update
```

Usuario inicial:

```text
Email: admin@escola.com
Senha: Senha@123
Perfil: Administrador
```

Endpoints principais:

- `POST /api/Auth/login`
- `GET /api/Auth/me`
- `GET /api/Aluno`
- `GET /api/Professor`
- `GET /api/Diretoria`
- `GET /api/usuarios`

No Swagger, use `POST /api/Auth/login`, copie o token e clique em `Authorize` informando `Bearer {token}`.

## Banco de dados

O banco principal e SQL Server. A API aplica migrations automaticamente no startup.

Tabelas principais:

- `Alunos`
- `Professores`
- `Diretoria`
- `Usuario`
- `Perfil`

Container usado no ambiente local atual:

```bash
docker network create form_api_net
docker run -d --name form_api_db --network form_api_net --network-alias db -p 14333:1433 \
  -e ACCEPT_EULA=Y \
  -e SA_PASSWORD=Your_password123 \
  -e MSSQL_PID=Express \
  mcr.microsoft.com/mssql/server:2022-latest
```

Aplicar migrations manualmente:

```bash
cd Backend_API
$env:ConnectionStrings__DefaultConnection='Server=localhost,14333;Database=FormDB;User Id=sa;Password=Your_password123;TrustServerCertificate=True;Encrypt=False'
dotnet ef database update
```

## Docker da API

```bash
docker build -t form-api:local Backend_API
docker run -d --name form_api_app --network form_api_net -p 8080:80 \
  -e ASPNETCORE_ENVIRONMENT=Development \
  -e ConnectionStrings__DefaultConnection="Server=db,1433;Database=FormDB;User Id=sa;Password=Your_password123;TrustServerCertificate=True;Encrypt=False" \
  form-api:local
```

Acessos:

- API: http://localhost:8080
- Swagger: http://localhost:8080/swagger
- SQL Server: `localhost,14333`

## Frontend

```bash
cd project_form
npm ci
npm run dev
```

Scripts principais:

```bash
npm run build
npm run generate
npm test
```

Variavel de ambiente:

```bash
NUXT_PUBLIC_API_BASE=http://localhost:8080/api
```

## CI/CD

Os workflows em `.github/workflows` usam os subdiretorios do mesmo repositorio:

- `Backend_API` para restore, build e testes .NET
- `project_form` para install, build/generate e testes unitarios Nuxt/Vitest
