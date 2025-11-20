# Ecommerce‑Tech (Fábrica de Projetos Ágeis IV)

Projeto de comércio eletrônico focado em produtos tecnológicos, desenvolvido como parte da Fábrica de Códigos IV.

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

### Frontend (Vue)
1. Acesse a pasta `Front-end`
2. Instale dependências:
   - `npm install` (ou `yarn`)
3. Configure a URL da API:
   - crie `.env` com `VITE_API_URL=http://localhost:8000/api`
4. Inicie o servidor de desenvolvimento:
   - `npm run dev`
   - Endereço padrão: `http://localhost:3000`

## 📚 Endpoints úteis
- Produtos: `GET /api/produto`, `POST /api/produto`, `PATCH /api/produto/{id}`, `DELETE /api/produto/{id}`
- Importação CSV (Admin): `POST /api/produto/import-csv`
- Categorias hierárquicas: `GET /api/categoria/hierarquia`

## 📋 Considerações finais
Este projeto demonstra um ecommerce especializado, aplicando boas práticas de arquitetura e integração entre frontend e backend. Ajuste variáveis de ambiente e conexões conforme seu ambiente de desenvolvimento.