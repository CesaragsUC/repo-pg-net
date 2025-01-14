
# 📦 PGNet - A PostgreSQL Repository for .NET

A generic repository with complete implementations for PostgreSQL using .NET.

✨ Description

This package provides a complete implementation of a generic repository for .NET applications with PostgreSQL, facilitating Create, Read, Update, and Delete (CRUD) operations for entities in the database.

With it, you can simplify data access using best practices, abstracting the repository layer and making your application cleaner and more decoupled.

🚀 Installation

You can install the package via NuGet Package Manager or the CLI:

Using NuGet Package Manager:
<pre> Install-Package RepoPgNet </pre>

🛠️ Configuration

```json
{
  "ConnectionStrings": {
    "PostgresConnection": "Host=localhost;Database=yourDB;Username=postgres;Password=yourpassword;"
  }
}
```

Configuring the DbContext:

```csharp
using Microsoft.EntityFrameworkCore;

namespace YourNamespace
{
    public class ProductPgDbContext : DbContext
    {
        public ProductPgDbContext(DbContextOptions<ProductPgDbContext> options) : base(options) { }

        // Add DbSets for your entities
        public DbSet<Product> Products { get; set; }
    }
}
```

In your Program.cs:

```csharp
using Microsoft.EntityFrameworkCore;
using YourNamespace;

var builder = WebApplication.CreateBuilder(args);

// Registering the repository and configuring the DbContext
builder.Services.AddRepoPgNet<ProductPgDbContext>(builder.Configuration);

var app = builder.Build();
```

🎯 Usage

Creating an Entity

Define an entity in your project:
```csharp
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}
```

Using the Repository

Example of using the generic repository in a Controller:

```csharp
public class ProductsController : ControllerBase
{
    private readonly IPgRepository<Product> _repository;

    public ProductsController(IPgRepository<Product> repository)
    {
        _repository = repository;
    }

    [HttpPost]
    public async Task<IActionResult> Create(Product product)
    {
        await _repository.AddAsync(product);
        return Ok("Product successfully created!");
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _repository.GetAllAsync();
        return Ok(products);
    }
}
```

⚙️ Features

Full CRUD:

* AddAsync(entity) - Adds a new entity.
* GetByIdAsync(id) - Retrieves an entity by ID.
* GetAllAsync() - Retrieves all entities.
* UpdateAsync(entity) - Updates an existing entity.
* DeleteAsync(id) - Removes an entity by ID.
* And much more..

Performance:

Efficient use of PostgreSQL database connections.

Generic:

Can be used with any entity class that has an identifier.

🧩 Requirements

* .NET 6+
* PostgreSQL 12+

🗂️ Package Structure

Interfaces:

``` IPgRepository<T>: Generic repository interface. ```

Implementations:

``` PgRepository<T>: Concrete implementation for PostgreSQL. ```

🤝 Contribution

Contributions are welcome!

* Fork the repository.
* Create a branch for your feature (git checkout -b feature/NewFeature).
* Commit your changes (git commit -m "Added a new feature X").
* Push to the branch (git push origin feature/NewFeature).
* Open a Pull Request.

⭐ Give it a Star!

If you found this package useful, don't forget to give it a ⭐ on GitHub!
