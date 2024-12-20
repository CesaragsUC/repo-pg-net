# 📦 PGNet a PostgreSQL Repository for .NET

Um repositório genérico com implementações completas para PostgreSQL usando .NET

✨ Descrição

Este pacote oferece uma implementação completa de um repositório genérico para aplicações .NET com PostgreSQL, facilitando a criação, leitura, atualização e remoção (CRUD) de entidades no banco de dados.

Com ele, você pode simplificar o acesso a dados usando boas práticas, abstraindo a camada de repositório e deixando sua aplicação mais limpa e desacoplada.

🚀 Instalação
Você pode instalar o pacote através do NuGet Package Manager ou da CLI:

Usando o NuGet Package Manager:
<pre> Install-Package SeuPackageName </pre>

🛠️ Configuração
```json
{
  "ConnectionStrings": {
    "PostgresConnection": "Host=localhost;Database=seuDB;Username=postgres;Password=suasenha;"
  }
}
```
Configurando DbContext:

```csharp
using Microsoft.EntityFrameworkCore;

namespace YourNamespace
{
    public class ProductPgDbContext : DbContext
    {
        public ProductPgDbContext(DbContextOptions<ProductPgDbContext> options) : base(options) { }

        // Adicione os DbSets de suas entidades
        public DbSet<Product> Products { get; set; }
    }
}

```
No seu Program.cs:

```csharp

using Microsoft.EntityFrameworkCore;
using YourNamespace;

var builder = WebApplication.CreateBuilder(args);

// Registrando o repositório e Configura o DbContext
builder.Services.AddRepoPgNet<ProductPgDbContext>(builder.Configuration);

var app = builder.Build();

```
🎯 Uso

Criando uma Entidade

Defina uma entidade no seu projeto:
```csharp
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}

```
Usando o Repositório

Exemplo de uso do repositório genérico no Controller:

```csharp
public class ProductsController : ControllerBase
{
    private readonly IPgRepository<Product> _repository;

    public ProductsController(IRepository<Product> repository)
    {
        _repository = repository;
    }

    [HttpPost]
    public async Task<IActionResult> Create(Product product)
    {
        await _repository.AddAsync(product);
        return Ok("Produto criado com sucesso!");
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _repository.GetAllAsync();
        return Ok(products);
    }
}

```


⚙️ Funcionalidades

CRUD Completo:

* AddAsync(entity) - Adiciona uma nova entidade.
* GetByIdAsync(id) - Retorna uma entidade pelo ID.
* GetAllAsync() - Retorna todas as entidades.
* UpdateAsync(entity) - Atualiza uma entidade existente.
* DeleteAsync(id) - Remove uma entidade pelo ID.
* Performance:

Uso eficiente de conexões com o banco de dados PostgreSQL.
Genérico:

Pode ser usado com qualquer classe de entidade que tenha um identificador.

🧩 Requisitos

* .NET 6 ou superior
* PostgreSQL 12+

🗂️ Estrutura do Pacote

Interfaces:

``` IPgRepository<T>: Interface do repositório genérico. ```
  
Implementações:

``` PgRepository<T>: Implementação concreta para PostgreSQL.```

🤝 Contribuição
Contribuições são bem-vindas!

* Faça um fork do repositório.
* Crie uma branch para sua feature (git checkout -b feature/NovaFeature).
* Commit suas mudanças (git commit -m "Adicionei uma nova feature X").
* Faça um push para a branch (git push origin feature/NovaFeature).
* Abra um Pull Request.

⭐ Dê uma estrela!

Se você achou este pacote útil, não se esqueça de dar uma ⭐ no GitHub!
