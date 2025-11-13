# 🛒 Sistema PDV - Ponto de Venda

Sistema completo de gerenciamento de vendas desenvolvido em C# com WPF, seguindo princípios de arquitetura em camadas e boas práticas de desenvolvimento.

![.NET](https://img.shields.io/badge/.NET-6.0-512BD4?logo=dotnet)
![WPF](https://img.shields.io/badge/WPF-Windows-0078D4?logo=windows)
![SQLite](https://img.shields.io/badge/SQLite-Database-003B57?logo=sqlite)
![Architecture](https://img.shields.io/badge/Architecture-Layered-green)

## 📸 Screenshots

_Adicione screenshots aqui_

---

## ✨ Funcionalidades

### 🔐 Autenticação e Segurança
- Sistema de login com hash de senha
- Controle de permissões (Administrador/Vendedor)
- Gestão completa de usuários

### 💰 PDV/Vendas
- Interface intuitiva de ponto de venda
- Busca rápida de produtos por código ou nome
- Adicionar/remover itens do carrinho
- Seleção de cliente (opcional)
- Baixa automática de estoque
- Atalhos de teclado (F2, F4, F5, F8, ESC)

### 📦 Gestão de Produtos
- CRUD completo (Create, Read, Update, Delete)
- Controle de estoque atual e mínimo
- Código de barras
- Busca e filtros em tempo real
- Status ativo/inativo

### 👥 Gestão de Clientes
- Cadastro completo de clientes
- CPF, telefone, email
- Histórico de compras

### 📊 Relatórios e Estatísticas
- Vendas por período customizável
- Top 10 produtos mais vendidos
- Produtos com estoque baixo
- Faturamento total
- Filtros por data

### 👤 Gestão de Usuários
- Tipos: Administrador e Vendedor
- Controle granular de permissões
- Alteração de senha
- Proteção do usuário admin

### ⚙️ Configurações
- Backup do banco de dados
- Restauração de backups
- Limpeza de dados antigos
- Otimização do banco (VACUUM)
- Configurações de comportamento do sistema

---

## 🏗️ Arquitetura

Este projeto implementa uma **Arquitetura em Camadas (Layered Architecture)**, seguindo os princípios de **Separação de Responsabilidades (SoC)** e **Inversão de Dependência**.

### 📐 Estrutura de Camadas

```
┌─────────────────────────────────────────────┐
│         CAMADA DE APRESENTAÇÃO              │
│           (SistemaPDV.UI)                   │
│    - Views (XAML)                           │
│    - Code-behind                            │
│    - Interação com usuário                  │
└────────────────┬────────────────────────────┘
                 │
                 ▼
┌─────────────────────────────────────────────┐
│        CAMADA DE NEGÓCIO                    │
│         (SistemaPDV.Business)               │
│    - Services                               │
│    - Regras de negócio                      │
│    - Validações                             │
│    - Lógica de aplicação                    │
└────────────────┬────────────────────────────┘
                 │
                 ▼
┌─────────────────────────────────────────────┐
│         CAMADA DE DADOS                     │
│          (SistemaPDV.Data)                  │
│    - Repositories                           │
│    - DbContext (EF Core)                    │
│    - Acesso ao banco de dados               │
└────────────────┬────────────────────────────┘
                 │
                 ▼
┌─────────────────────────────────────────────┐
│         CAMADA DE MODELOS                   │
│         (SistemaPDV.Models)                 │
│    - Entidades                              │
│    - DTOs                                   │
│    - Modelos de dados                       │
└─────────────────────────────────────────────┘
```

### 📦 Detalhamento das Camadas

#### 1️⃣ **SistemaPDV.Models** (Camada de Modelos)
**Responsabilidade:** Definir as estruturas de dados

- **Entidades:** Usuario, Produto, Cliente, Venda, ItemVenda
- **Annotations:** Data Annotations para validação
- **Relacionamentos:** Definição de FKs e navegação

**Exemplo:**
```csharp
public class Produto
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public decimal Preco { get; set; }
    public int EstoqueAtual { get; set; }
}
```

#### 2️⃣ **SistemaPDV.Data** (Camada de Acesso a Dados)
**Responsabilidade:** Comunicação com o banco de dados

- **DbContext:** Configuração do Entity Framework Core
- **Repositories:** Implementação do padrão Repository
- **Migrations:** Versionamento do schema do banco

**Padrões Implementados:**
- ✅ **Repository Pattern:** Abstração do acesso a dados
- ✅ **Unit of Work:** Controle de transações

**Exemplo:**
```csharp
public class BaseRepository<T> where T : class
{
    public async Task<List<T>> GetAllAsync()
    public async Task<T> GetByIdAsync(int id)
    public async Task<T> AddAsync(T entity)
    public async Task<T> UpdateAsync(T entity)
    public async Task<bool> DeleteAsync(int id)
}
```

#### 3️⃣ **SistemaPDV.Business** (Camada de Negócio)
**Responsabilidade:** Lógica de negócio e regras da aplicação

- **Services:** AutenticacaoService, VendaService
- **Validações:** Regras de negócio
- **Transações:** Operações complexas (ex: venda com baixa de estoque)

**Exemplo:**
```csharp
public class VendaService
{
    public async Task<Venda> RealizarVendaAsync(...)
    {
        // 1. Validar estoque
        // 2. Criar venda
        // 3. Adicionar itens
        // 4. Baixar estoque
        // 5. Commit da transação
    }
}
```

#### 4️⃣ **SistemaPDV.UI** (Camada de Apresentação)
**Responsabilidade:** Interface com o usuário

- **Views:** Telas XAML (LoginWindow, MainWindow, etc)
- **Code-behind:** Lógica de UI
- **MVVM parcial:** ViewModels onde aplicável

**Princípios:**
- ✅ Separação de código UI e lógica de negócio
- ✅ Binding de dados
- ✅ Commands e eventos

---

## 🎯 Padrões de Projeto Utilizados

### 1. **Repository Pattern**
Abstração da camada de dados, facilitando testes e manutenção.

```csharp
var produto = await _produtoRepository.GetByIdAsync(id);
```

### 2. **Dependency Injection (DI)**
Injeção de dependências via construtores.

```csharp
public VendaService(AppDbContext context)
{
    _context = context;
}
```

### 3. **MVVM (Model-View-ViewModel)** - Parcial
Implementado em algumas telas mais complexas.

### 4. **Unit of Work**
Controle de transações para operações atômicas.

```csharp
using var transaction = await _context.Database.BeginTransactionAsync();
try 
{
    // Operações
    await transaction.CommitAsync();
}
catch 
{
    await transaction.RollbackAsync();
}
```

---

## 🚀 Tecnologias e Ferramentas

### Backend
- **C# 10** - Linguagem de programação
- **.NET 6.0** - Framework
- **Entity Framework Core 7.0** - ORM
- **SQLite** - Banco de dados

### Frontend
- **WPF** - Windows Presentation Foundation
- **XAML** - Interface declarativa
- **Data Binding** - Binding bidirecional

### Ferramentas de Desenvolvimento
- **Visual Studio 2022** - IDE
- **Git** - Controle de versão
- **NuGet** - Gerenciamento de pacotes

---

## 📂 Estrutura do Projeto

```
SistemaPDV/
│
├── SistemaPDV.sln                      # Solution principal
│
├── SistemaPDV.Models/                  # 📊 Camada de Modelos
│   └── Entities/
│       ├── Usuario.cs
│       ├── Produto.cs
│       ├── Cliente.cs
│       ├── Venda.cs
│       └── ItemVenda.cs
│
├── SistemaPDV.Data/                    # 🗄️ Camada de Dados
│   ├── Context/
│   │   └── AppDbContext.cs
│   ├── Repositories/
│   │   ├── BaseRepository.cs
│   │   └── ProdutoRepository.cs
│   └── Migrations/
│
├── SistemaPDV.Business/                # 💼 Camada de Negócio
│   └── Services/
│       ├── AutenticacaoService.cs
│       └── VendaService.cs
│
└── SistemaPDV.UI/                      # 🖥️ Camada de Apresentação
    ├── Views/
    │   ├── LoginWindow.xaml
    │   ├── MainWindow.xaml
    │   ├── ProdutosWindow.xaml
    │   ├── VendasWindow.xaml
    │   ├── ClientesWindow.xaml
    │   ├── RelatoriosWindow.xaml
    │   ├── UsuariosWindow.xaml
    │   └── ConfiguracoesWindow.xaml
    └── App.xaml
```

---

## 🔧 Como Executar

### Pré-requisitos

- **Visual Studio 2022** ou superior
- **.NET 6.0 SDK** ou superior
- **Git** (opcional)

### Passo a Passo

1. **Clone o repositório:**
```bash
git clone https://github.com/SEU-USUARIO/SistemaPDV.git
cd SistemaPDV
```

2. **Abra a solution no Visual Studio:**
```
SistemaPDV.sln
```

3. **Restaure os pacotes NuGet:**
   - O Visual Studio faz automaticamente
   - Ou via terminal: `dotnet restore`

4. **Execute as migrations:**
   - Abra o **Package Manager Console**
   - Selecione **SistemaPDV.Data** como projeto padrão
   - Execute:
```powershell
Add-Migration InitialCreate
Update-Database
```

5. **Execute o projeto:**
   - Pressione **F5** ou clique em ▶️ **Start**

6. **Faça login:**
   - **Usuário:** `admin`
   - **Senha:** `admin123`

---

## 📦 Pacotes NuGet Utilizados

### SistemaPDV.Data
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="7.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="7.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="7.0.0" />
```

### SistemaPDV.UI
```xml
<PackageReference Include="Microsoft.VisualBasic" Version="10.3.0" />
```

---

## 🎓 Conceitos Aplicados

### SOLID Principles

- ✅ **S**ingle Responsibility Principle
  - Cada camada tem uma responsabilidade única
  
- ✅ **O**pen/Closed Principle
  - Classes abertas para extensão, fechadas para modificação
  
- ✅ **L**iskov Substitution Principle
  - Repositories podem ser substituídos

- ✅ **I**nterface Segregation Principle
  - Interfaces específicas para cada contexto

- ✅ **D**ependency Inversion Principle
  - Dependências via abstração (repositories)

### Clean Code

- ✅ Nomes descritivos
- ✅ Métodos pequenos e focados
- ✅ Comentários apenas onde necessário
- ✅ Tratamento adequado de exceções

### Async/Await

- ✅ Todas operações de I/O são assíncronas
- ✅ Melhor performance e responsividade

---

## 🔒 Segurança

- ✅ Senhas com hash (Base64 - **Trocar por BCrypt em produção**)
- ✅ Controle de permissões por tipo de usuário
- ✅ Validação de entrada em todos os formulários
- ✅ Proteção contra exclusão acidental (confirmações)

### ⚠️ Recomendação de Produção

Para ambiente de produção, substitua o hash simples por **BCrypt**:

```bash
Install-Package BCrypt.Net-Next
```

```csharp
// Hash
var senhaHash = BCrypt.Net.BCrypt.HashPassword(senha);

// Validar
bool senhaCorreta = BCrypt.Net.BCrypt.Verify(senhaDigitada, senhaHash);
```

---

## 📊 Banco de Dados

### Diagrama ER

```
┌─────────────┐       ┌──────────────┐
│   Usuario   │       │   Cliente    │
├─────────────┤       ├──────────────┤
│ Id (PK)     │       │ Id (PK)      │
│ Nome        │       │ Nome         │
│ Login       │       │ CPF          │
│ SenhaHash   │       │ Telefone     │
│ Tipo        │       │ Email        │
│ Ativo       │       │ Ativo        │
└──────┬──────┘       └──────┬───────┘
       │                     │
       │                     │
       ▼                     ▼
┌─────────────────────────────────┐
│           Venda                 │
├─────────────────────────────────┤
│ Id (PK)                         │
│ DataVenda                       │
│ UsuarioId (FK) ────────────────►│
│ ClienteId (FK) ─────────────────►│
│ ValorTotal                      │
└────────────┬────────────────────┘
             │
             │ 1:N
             ▼
┌─────────────────────────────────┐
│         ItemVenda               │
├─────────────────────────────────┤
│ Id (PK)                         │
│ VendaId (FK)                    │
│ ProdutoId (FK) ─────────────────┐
│ Quantidade                      │
│ PrecoUnitario                   │
└─────────────────────────────────┘
                                  │
                                  ▼
                         ┌──────────────┐
                         │   Produto    │
                         ├──────────────┤
                         │ Id (PK)      │
                         │ Nome         │
                         │ CodigoBarras │
                         │ Preco        │
                         │ EstoqueAtual │
                         │ Ativo        │
                         └──────────────┘
```

---

## 📝 TODO / Melhorias Futuras

### Segurança
- [ ] Implementar BCrypt para hash de senhas
- [ ] Adicionar autenticação JWT
- [ ] Log de auditoria de ações

### Funcionalidades
- [ ] Gráficos interativos nos relatórios (Chart.js / LiveCharts)
- [ ] Exportar relatórios para Excel/PDF
- [ ] Impressão de cupom fiscal
- [ ] Integração com TEF (pagamento com cartão)
- [ ] Emissão de NFC-e
- [ ] Suporte a múltiplas lojas
- [ ] Dashboard web (ASP.NET Core + React)

### Técnicas
- [ ] Implementar CQRS
- [ ] Adicionar testes unitários
- [ ] CI/CD com GitHub Actions
- [ ] Containerização com Docker
- [ ] Migrar para Blazor (versão web)

---

## 🤝 Contribuindo

Contribuições são bem-vindas! Para contribuir:

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/NovaFuncionalidade`)
3. Commit suas mudanças (`git commit -m 'Adiciona nova funcionalidade'`)
4. Push para a branch (`git push origin feature/NovaFuncionalidade`)
5. Abra um Pull Request

---



## 👤 Autor

**Celso Antunes Nogueira**

- 💼 LinkedIn: (https://www.linkedin.com/in/celsoantunesnogueira/)
- 🐙 GitHub: (https://github.com/CelsoAntunesNogueira)
- 📧 Email: scelsoa81@gmail.com

---

## 🙏 Agradecimentos

- Documentação oficial do [.NET](https://docs.microsoft.com/dotnet/)
- Comunidade [WPF](https://github.com/dotnet/wpf)
- Padrões de arquitetura: [Martin Fowler](https://martinfowler.com/)

---

## ⭐ Se este projeto te ajudou, deixe uma estrela!

[![GitHub stars](https://img.shields.io/github/stars/SEU-USUARIO/SistemaPDV?style=social)](https://github.com/SEU-USUARIO/SistemaPDV/stargazers)
[![GitHub forks](https://img.shields.io/github/forks/SEU-USUARIO/SistemaPDV?style=social)](https://github.com/SEU-USUARIO/SistemaPDV/network/members)

---

