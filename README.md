# Ecommerce‑Tech (Fábrica de Projetos Ágeis IV)

Projeto de comércio de moda focado em produtos como camisas e calçados, desenvolvido como parte da Fábrica de Códigos IV.

## 🛠 Tecnologias
- Frontend: Vue.js (Vite)
- Backend: .NET / C# (ASP.NET Core)
- API REST entre frontend e backend
- Estrutura modular com camadas: controllers, serviços, repositórios, modelos

## 🚀 Funcionalidades principais
- Gerenciamento de produtos (CRUD) e variações
- Autenticação e autorização de usuários
- Carrinho de compras
- Finalização de pedidos (checkout)
- Visualização de catálogos e filtros por categorias hierárquicas
- Integração completa entre frontend e backend

## 🏗 Estrutura do projeto
```
/
├─ Front-end/     ← cliente (Vue.js + Vite)
├─ Back-end/      ← API (ASP.NET Core)
└─ README.md      ← este arquivo
```

## 💡 Como rodar localmente

### Backend (API)
1. Acesse a pasta `Back-end`
2. Configure a string de conexão no `appsettings.json`
3. Instale/restaure os pacotes:
   - `dotnet restore`
4. Execute a API:
   - `dotnet run`
- Endereço padrão: `http://localhost:8000` (Swagger habilitado)

### 🔐 Configuração de e‑mail (SMTP)

- Use variáveis de ambiente para não expor segredos:

```
Email__SmtpHost=smtp.gmail.com
Email__Port=587
Email__EnableSsl=true
Email__User=dunderstoreofc@gmail.com
Email__From=dunderstoreofc@gmail.com
Email__FromName=Dunder Store
Email__Password=SENHA_DE_APP_GMAIL
```

- Windows PowerShell (exemplo):

```
$env:Email__SmtpHost='smtp.gmail.com'
$env:Email__Port='587'
$env:Email__EnableSsl='true'
$env:Email__User='dunderstoreofc@gmail.com'
$env:Email__From='dunderstoreofc@gmail.com'
$env:Email__FromName='Dunder Store'
$env:Email__Password='SUA_SENHA_DE_APP'
dotnet run --project .\Back-end\Dunder_Store.csproj
```

### Frontend (Vue)
1. Acesse a pasta `Front-end`
2. Instale dependências:
   - `npm install` (ou `yarn`)
3. Configure a URL da API:
   - crie `.env` com `VITE_API_URL=http://localhost:8000/api`
4. Inicie o servidor de desenvolvimento:
   - `npm run dev`
- Endereço padrão: `http://localhost:3000`

## ✅ Testes e relatórios

- Back‑end (xUnit):

```
dotnet test .\Back-end\Tests\Unit\Dunder_Store.UnitTests.csproj --no-restore --no-build --logger html; start .\Back-end\Tests\Unit\TestResult.html
dotnet test .\Back-end\Tests\Integration\Dunder_Store.IntegrationTests.csproj --no-restore --no-build --logger html; start .\Back-end\Tests\Integration\TestResult.html
```

- Front‑end (Playwright):

```
cd Front-end
npx playwright test --reporter=html && npx playwright show-report
```

- Observações:
  - Relatórios HTML são abertos automaticamente com `start` (Windows).
  - Artefatos de Playwright (`playwright-report/`, `test-results/`) já estão ignorados via `.gitignore`.

## 📚 Endpoints úteis
- Produtos: `GET /api/produto`, `POST /api/produto`, `PATCH /api/produto/{id}`, `DELETE /api/produto/{id}`
- Categorias hierárquicas: `GET /api/categoria/hierarquia`

## 📋 Considerações finais
Este projeto demonstra um ecommerce especializado, aplicando boas práticas de arquitetura e integração entre frontend e backend. Ajuste variáveis de ambiente e conexões conforme seu ambiente de desenvolvimento.
