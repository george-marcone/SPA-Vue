# Frontend - Arquitetura e regras de negocio

Data: 21/05/2026

## Visao geral

O frontend fica em `ESCOLA_FRONT` e foi preparado como uma SPA em Nuxt 3. O Nuxt usa Vite no ambiente de desenvolvimento e no build, Pinia para estado global e Vitest para testes unitarios.

A aplicacao conversa com o backend `Backend_API` por HTTP usando JWT. O token recebido no login e salvo pelo store de autenticacao e enviado nas proximas requisicoes pelo plugin de API.

## Tecnologias

- Nuxt 3 com `ssr: false`.
- Vite como bundler do Nuxt.
- Pinia e `@pinia/nuxt` para estado global.
- Vitest, Vue Test Utils e Happy DOM para testes.
- TypeScript em modo estrito.
- CSS global em `assets/css/main.css`.
- GitHub Actions para build, testes, geracao estatica e deploy no GitHub Pages.

## Estrutura principal

- `pages/login.vue`: entrada do usuario, autenticacao JWT e troca da senha padrao apos o primeiro login.
- `pages/alterar-senha.vue`: tela autenticada para alterar senha a qualquer momento.
- `pages/usuarios/novo.vue`: cadastro de usuario sem campo de senha.
- `pages/diretoria/novo.vue`: cadastro da tabela Diretoria.
- `stores/auth.ts`: sessao, usuario logado, token, flag de senha padrao e acao de alteracao de senha.
- `middleware/auth.global.ts`: protecao de rotas e bloqueio de navegacao enquanto a senha padrao nao for alterada.
- `components/PasswordStrengthMeter.vue`: medidor visual de forca de senha.
- `utils/password-strength.ts`: regra reutilizavel para classificar senhas.
- `types/api.ts`: contratos TypeScript usados pelo frontend.

## Regra de negocio - cadastro de usuario

O cadastro de usuario nao exibe mais o campo de senha. A tela envia apenas dados cadastrais, como nome, email, telefone e perfil.

No backend, o `UsuarioService` cria a senha automaticamente usando o valor padrao:

```text
Senha@252525
```

Essa senha nao e salva em texto puro. O backend aplica hash PBKDF2-SHA256 pelo `PasswordHasher`.

## Regra de negocio - troca obrigatoria da senha padrao

Quando o usuario faz login, o backend valida email e senha e devolve o JWT. Na mesma resposta, tambem devolve a flag:

```json
{
  "deveAlterarSenhaPadrao": true
}
```

Se essa flag vier como `true`, o frontend mantem o usuario autenticado, mas bloqueia a navegacao normal e exibe a tela de troca de senha.

O usuario deve informar:

- senha atual;
- nova senha;
- confirmacao da nova senha.

A nova senha deve ser diferente da senha atual e nao pode continuar sendo `Senha@252525`.

Depois que a senha e alterada com sucesso, o Pinia limpa a flag `deveAlterarSenhaPadrao` e libera a navegacao.

## Medidor de forca de senha

O medidor de forca usa `utils/password-strength.ts`. A classificacao considera:

- tamanho minimo;
- letras minusculas;
- letras maiusculas;
- numeros;
- caracteres especiais.

As faixas exibidas sao:

- nao informada;
- fraca;
- media;
- forte.

O componente `PasswordStrengthMeter.vue` mostra uma barra visual, o rotulo da forca e dicas para melhorar a senha.

## Autenticacao e estado

O store `stores/auth.ts` concentra a sessao:

- token JWT;
- dados resumidos do usuario;
- data de expiracao;
- indicador `deveAlterarSenhaPadrao`;
- acoes `login`, `logout`, `fetchMe` e `alterarSenha`.

A sessao e persistida no `localStorage` para sobreviver ao recarregamento da pagina. O middleware global usa esse estado para proteger rotas autenticadas.

## Integracao com o backend

Principais endpoints usados pelo frontend:

- `POST /api/Auth/login`;
- `GET /api/Auth/me`;
- `POST /api/Auth/alterar-senha`;
- `POST /api/usuarios`;
- `POST /api/Diretoria`.

O endpoint de alteracao de senha exige JWT valido.

## Protecao contra SQL injection

No backend, as consultas usam Entity Framework Core com LINQ. Os valores de entrada sao enviados como parametros pelo provider do EF Core, evitando concatenacao manual de SQL.

Tambem foram adicionadas validacoes com FluentValidation nos contratos de entrada.

Para prevenir regressao, existe um teste automatizado em `Backend_API/form_API.Tests/Security/SqlInjectionProtectionTests.cs`. Esse teste falha se APIs de SQL bruto forem introduzidas no codigo da aplicacao, como `FromSqlRaw`, `ExecuteSqlRaw` ou `SqlQueryRaw`.

## Testes

Frontend:

```bash
cd ESCOLA_FRONT
npm test
```

Backend:

```bash
cd Backend_API
dotnet test form_API.Tests/form_API.Tests.csproj
```

Coberturas adicionadas:

- store de autenticacao;
- troca de senha no Pinia;
- medidor de forca de senha;
- regra backend da senha padrao;
- validacao da nova senha;
- protecao contra SQL bruto.

## CI/CD do frontend

O workflow `.github/workflows/frontend-pages.yml` executa um job especifico para o front:

1. checkout do repositorio;
2. setup do Node.js 22;
3. `npm ci`;
4. `npm test`;
5. `npm run generate`;
6. upload do artefato estatico;
7. deploy no GitHub Pages quando nao for pull request.

Como o frontend esta em uma subpasta do mesmo repositorio, o job usa `working-directory: ESCOLA_FRONT`.

Para GitHub Pages, `NUXT_APP_BASE_URL` e definido como `/{nome-do-repositorio}/`. A URL da API deve ser configurada em `NUXT_PUBLIC_API_BASE` nas variaveis ou secrets do repositorio.

## Como rodar localmente

Backend:

```bash
cd Backend_API
dotnet run
```

Frontend:

```bash
cd ESCOLA_FRONT
npm ci
$env:NUXT_PUBLIC_API_BASE='http://localhost:5001/api'
npm run dev
```

Para gerar a versao estatica:

```bash
cd ESCOLA_FRONT
npm run generate
```
