# Desafio Técnico — Desenvolvedor C# - STgenetics

## 🍔 Good Hamburger — Sistema de Pedidos

Sistema completo de gestão de pedidos para lanchonete, desenvolvido como demonstração de **arquitetura limpa** em ASP.NET Core + Blazor, este projeto
foi desenvolvido como teste para fazer parte da equipa de desenvolvedores da empresa STgenetics.

---

## 🚀 Como Executar

### Pré-requisitos
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### 1. Clonar o repositório
```bash
git clone https://github.com/lucasmudile/TesteSTgenetics.git
cd TesteSTgenetics
```

### 2. Iniciar a API (terminal 1)
```bash
cd src/GoodHamburger.Api
dotnet run --launch-profile https
```
API disponível em: **https://localhost:7000**  
Swagger UI: **https://localhost:7000/swagger**

### 3. Iniciar o Frontend Blazor (terminal 2)
```bash
cd src/GoodHamburger.Web
dotnet run --launch-profile https
```
Frontend disponível em: **https://localhost:7001**

### 4. Executar os Testes
```bash
dotnet test tests/GoodHamburger.Tests
```

---

## 🚀 Como Executar o projeto segunda opção

```bash
 Clonar o repositório
 git clone https://github.com/lucasmudile/TesteSTgenetics.git
 Abrir a pasta TesteSTgenetic, abrir o ficheiro GoodHamburger.sln com o visual studio.
 Clicar com o botão direito do mouse por cima da solution (GoodHamburger)
 Clicar na opção Configure Startup Projects
 Selecionar a opção Multiple startup projects
 Na coluna Action escolher a opção em Start para GoodHamburger.Api e  GoodHamburger.Web
 Clicar no botão Aplicar e depois Ok
 Agora é só clicar na opção Start.
```

## 📋 Cardápio

| Item | Preço | Categoria |
|---|---|---|
| X Burger | R$ 5,00 | Sanduíche |
| X Egg | R$ 4,50 | Sanduíche |
| X Bacon | R$ 7,00 | Sanduíche |
| Batata Frita | R$ 2,00 | Acompanhamento |
| Refrigerante | R$ 2,50 | Acompanhamento |

### Regras de Desconto

| Combinação | Desconto |
|---|---|
| Sanduíche + Batata + Refrigerante | 20% |
| Sanduíche + Refrigerante | 15% |
| Sanduíche + Batata Frita | 10% |
| Outras combinações | 0% |

---

## 🔌 Endpoints da API

| Método | URL | Descrição |
|---|---|---|
| `GET` | `/api/menu` | Retorna o cardápio completo |
| `GET` | `/api/orders` | Lista todos os pedidos |
| `GET` | `/api/orders/paged` | Lista os pedidos com paginação e filtros |
| `POST` | `/api/orders` | Cria um novo pedido |
| `GET` | `/api/orders/{id}` | Consulta pedido por ID |
| `PUT` | `/api/orders/{id}` | Atualiza itens de um pedido |
| `DELETE` | `/api/orders/{id}` | Remove um pedido |

### Exemplo: Criar Pedido
```http
POST https://localhost:7000/api/orders
Content-Type: application/json

{
  "items": [1, 4, 5]
}
```
Códigos: `1=XBurger`, `2=XEgg`, `3=XBacon`, `4=BatataFrita`, `5=Refrigerante`

### Exemplo de Resposta
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "createdAt": "2025-04-21T00:00:00Z",
  "updatedAt": "2025-04-21T00:00:00Z",
  "items": [
    { "code": 1, "name": "X Burger", "price": 5.00, "type": "Sandwich" },
    { "code": 4, "name": "Batata Frita", "price": 2.00, "type": "Side" },
    { "code": 5, "name": "Refrigerante", "price": 2.50, "type": "Side" }
  ],
  "subtotal": 9.50,
  "discountPercentage": 20,
  "discountAmount": 1.90,
  "total": 7.60
}
```

---

## 🏗️ Arquitetura e Decisões Técnicas

### Estrutura do Projeto
```
GoodHamburger/
├── src/
│   ├── GoodHamburger.Api/          ← API REST (ASP.NET Core 8)
│   │   ├── Domain/
│   │   │   ├── Entities/           ← Order, MenuItem (rich domain)
│   │   │   ├── Enums/              ← MenuItemType, MenuItemCode
│   │   │   └── Catalog/            ← Menu estático (fonte de verdade)
│   │   ├── Application/
│   │   │   ├── DTOs/               ← Contratos da API (records)
│   │   │   ├── Interfaces/         ← IOrderRepository
│   │   │   └── Services/           ← OrderService (lógica de negócio)
│   │   ├── Infrastructure/
│   │   │   └── Repositories/       ← InMemoryOrderRepository
│   │   └── Controllers/            ← OrdersController, MenuController
│   └── GoodHamburger.Web/          ← Frontend Blazor
│       ├── Pages/                  ← Home, MenuPage, Orders, Create, Edit
│       ├── Services/               ← ApiService (HttpClient)
│       └── Models/                 ← DTOs do frontend
└── tests/
    └── GoodHamburger.Tests/        ← xUnit + FluentAssertions
```

### Decisões de Arquitetura

**1. Regras de desconto na entidade `Order`**  
O cálculo de desconto fica encapsulado em `Order.CalculateDiscountPercentage()` usando C# pattern matching. Isto garante que a regra nunca pode ser contornada e é facilmente testável.

**2. Catálogo estático em `Menu.cs`**  
Como os preços não mudam durante a execução, o cardápio é um objeto estático imutável. Evita um repositório de cardápio desnecessário. Em produção, seria uma tabela de BD com cache.

**3. Repositório In-Memory com `ConcurrentDictionary`**  
Thread-safe sem dependências externas. O padrão Repository garante que trocar para EF Core + SQL Server é uma mudança de uma única classe.

**4. DTOs como `record`**  
Os DTOs de resposta são imutáveis por defeito. Separam completamente o contrato HTTP do domínio.

**5. Frontend Blazor**  
Corre inteiramente no browser. O `ApiService` é o único ponto de contacto com a API. O preview de desconto no formulário recalcula os mesmos valores que a API calcularia, dando feedback imediato ao utilizador.

**6. Validação de erros**  
- Itens duplicados → `400 Bad Request` com mensagem clara
- Dois sanduíches → `400 Bad Request`
- ID inexistente → `404 Not Found`
- Todas as respostas de erro usam `ProblemDetails` (RFC 7807)

### O que ficou de fora (e porquê)
- **Base de dados persistente**: Optei por in-memory para zero dependências externas e por ser um teste. ,Em produção: EF Core + SQL Server e a estrutura do projeto já está prota caso queira usar o EF.
- **Autenticação/JWT**: Fora do escopo do teste, não vi necessidade de utilizar pós seria aumentar complexidade uma vez que não temos utilizador no sistema e o teste não menciona ter.
- **Docker**: Não vi necessidade em usar docker para este projeto.

---

## 🧪 Cobertura de Testes (15 testes)

| Categoria | Teste |
|---|---|
| Desconto | 20% — combo completo |
| Desconto | 15% — sanduíche + refrigerante |
| Desconto | 10% — sanduíche + batata |
| Desconto | 0% — só sanduíche |
| Desconto | 0% — só acompanhamento |
| Validação | Itens duplicados |
| Validação | Dois sanduíches diferentes |
| Validação | Pedido vazio |
| Validação | Duas batatas fritas |
| CRUD | Listar após criar |
| CRUD | Consultar por ID |
| CRUD | ID inexistente → KeyNotFoundException |
| CRUD | Atualizar recalcula desconto |
| CRUD | Remover pedido |
| CRUD | Remover ID inexistente |
