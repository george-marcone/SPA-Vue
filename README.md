# SPA-Vue

Aplicacao escolar com backend ASP.NET Core e frontend Nuxt 3 em modo SPA.

## Atualizacoes recentes

- Frontend em `project_form` usando Nuxt 3, Vite, Pinia e Vitest.
- Login com JWT, sessao persistida no Pinia e middleware de rota autenticada.
- Cadastro de usuarios sem campo de senha.
- Novos usuarios recebem a senha inicial padrao `Senha@252525`.
- Quando o usuario entra com a senha padrao, o sistema exige a troca antes de liberar as demais telas.
- Tela de alteracao de senha com medidor de forca.
- Backend com endpoint protegido `POST /api/Auth/alterar-senha`.
- Protecao contra SQL injection por consultas LINQ/EF Core parametrizadas, validacao de entrada e teste automatizado contra uso de SQL bruto.
- Workflow separado para CI/CD do frontend e publicacao estatica no GitHub Pages.

## Documentacao

- Documentacao tecnica do backend: [docs/backend-tecnico.md](docs/backend-tecnico.md)
- Documentacao do frontend: [docs/frontend-arquitetura.md](docs/frontend-arquitetura.md)
- PDF do frontend: [docs/frontend-arquitetura.pdf](docs/frontend-arquitetura.pdf)
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

Usuario inicial de seed:

```text
Email: admin@escola.com
Senha: Senha@123
Perfil: Administrador
```

Novos usuarios cadastrados pela tela de usuario recebem:

```text
Senha inicial: Senha@252525
```

Endpoints principais:

- `POST /api/Auth/login`
- `GET /api/Auth/me`
- `POST /api/Auth/alterar-senha`
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

Variaveis de ambiente:

```bash
NUXT_PUBLIC_API_BASE=http://localhost:8080/api
NUXT_APP_BASE_URL=/
```

Para GitHub Pages, o workflow define `NUXT_APP_BASE_URL` como `/{nome-do-repositorio}/`.

## CI/CD

Os workflows em `.github/workflows` usam os subdiretorios do mesmo repositorio:

- `ci-backend-frontend.yml`: build e testes do backend e do frontend em jobs separados.
- `frontend-pages.yml`: CI do frontend e deploy estatico no GitHub Pages.
- `master_spa-vue.yml`: build do frontend e deploy manual para Azure Web App.

Para publicar no GitHub Pages, configure `NUXT_PUBLIC_API_BASE` como variavel ou secret do repositorio apontando para a API publicada.
