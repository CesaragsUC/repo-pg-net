
# 📦 HybridRepoNet  - A Hybrid Repository for .NET  
A generic repository with a **Unit of Work** pattern and domain event handling for **PostgreSQL** and **Sql Server** using .NET.

## ✨ Description  

**HybridRepoNet** is a robust and extensible repository implementation for .NET applications using **PostgreSQL** and **Sql Server**. It simplifies **Create, Read, Update, and Delete (CRUD)** operations while maintaining a **clean architecture** through the **Unit of Work (UoW) pattern** and **Domain Events**.

With this package, you can:  
- Abstract the data access layer using the **Repository Pattern**.  
- Manage transactions efficiently with **Unit of Work**.  
- Automatically dispatch **Domain Events** on entity changes (e.g., create, update, delete).  
- Keep your code **clean, decoupled, and scalable**.
- You can you PostgreSQL ans Sql Server at same time in your aplication.

This approach enhances **maintainability** and **testability**, following best practices in **DDD (Domain-Driven Design)**.  

## 🚀 Installation  

You can install the package via NuGet Package Manager or the CLI:

Using NuGet Package Manager:
<pre> Install-Package RepoPgNet </pre>

🛠️ Configuration

```json
{
  "ConnectionStrings": {
    "PostgresConnection": "Host=localhost;Database=yourDB;Username=postgres;Password=yourpassword;",
    "SqlConnection": "Server=(localdb)\\mssqllocaldb;Database=Cars;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

Configuring the DbContext:

```csharp
using Microsoft.EntityFrameworkCore;

namespace YourNamespace
{
    public class ProductPostgreSqlContext : DbContext
    {
        public ProductPgDbContext(DbContextOptions<ProductPgDbContext> options) : base(options) { }

        // Add DbSets for your entities
        public DbSet<Product> Products { get; set; }
    }
}
namespace YourNamespace
{
    public class CarSqlServerContext : DbContext
    {
        public CarSqlServerContext(DbContextOptions<CarSqlServerContext> options) : base(options) { }

        // Add DbSets for your entities
        public DbSet<Car> Cars { get; set; }
    }
}
```

In your Program.cs:

```csharp
using Microsoft.EntityFrameworkCore;
using YourNamespace;

var builder = WebApplication.CreateBuilder(args);

// Registering the repository and configuring the DbContext
builder.Services.AddRepoPgNet<ProductPostgreSqlContext>(builder.Configuration, DbType.PostgreSQL);
builder.Services.AddRepoPgNet<CarSqlServerContext>(builder.Configuration, DbType.SQLServer);

services.AddMediatR(cfg => {
 //Register MediatR handlers
 cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly());
});

var app = builder.Build();
```

🎯 Usage

Creating an Entity

Define an entity in your project, this entity inherit BaseEntity that is a base class wich contains all domain events implementantion:
```csharp
public class Product : BaseEntity
{
    public string? Name { get; set; }

    public decimal Price { get; set; }

    public bool Active { get; set; }

    public string? ImageUri { get; set; }
}

```

Using the Repository

Example of usage:

```csharp
public class ProductService : IProductService
{
    private readonly IUnitOfWork<ProductContext> _unitOfWork;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    public ProductService(IUnitOfWork<ProductContext> unitOfWork,
        IMapper mapper,
        IProductRepository productRepository)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _productRepository = productRepository;
    }

    public async Task<IEnumerable<ProductDto>> GetAll()
    {
        return  await _unitOfWork.Repository<Product>()
            .Entities.ProjectTo<ProductDto>(_mapper.ConfigurationProvider).ToListAsync();

    }

    public async Task<ProductDto> Get(Guid id)
    {
        var product = _unitOfWork.Repository<Product>().FindOne(x => x.Id == id);
        return _mapper.Map<ProductDto>(product);
    }

    public async Task<ProductDto> GetByName(string name)
    {
        return await _productRepository.GetByName(name);
    }

    public async Task Add(ProductCreateDto productDto)
    {
        var product = _mapper.Map<Product>(productDto);
        await _unitOfWork.Repository<Product>().AddAsync(product);

        // send a event
        product.AddDomainEvent(new ProductCreatedEvent(product));

        var resul = await _unitOfWork.Commit();
    }

    public async Task Delete(Guid id)
    {
        var product = _unitOfWork.Repository<Product>().FindOne(x => x.Id == id);

        if (product is null)
            throw new Exception("Product not found");

        _unitOfWork.Repository<Product>().DeleteAsync(product);
        await _unitOfWork.Commit();
    }

    public async Task Update(ProductUpdateDto productDto)
    {
        var product = _unitOfWork.Repository<Product>().FindOne(x => x.Id == productDto.Id);

        if (product is null)
            throw new Exception("Product not found");

        product.Name = productDto.Name;
        product.Price = productDto.Price;
        product.Active = productDto.Active;
        product.ImageUri = productDto.ImageUri;

        _unitOfWork.Repository<Product>().UpdateAsync(product);
        await _unitOfWork.Commit();
    }
}

```

## ✨ Using Domain Events  
Assuming that you have MediatR installed in your project, you can create your Handler for a created Product. Here a example

```csharp
public class ProductCreatedEvent : BaseEvent
{
    public Product Product { get;}
    public ProductCreatedEvent(Product product)
    {
        Product = product;
    }
}

public class ProductCreatedEventHandler : INotificationHandler<ProductCreatedEvent>
{
    public async Task Handle(ProductCreatedEvent notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Product {notification.Product.Name} created at {DateTime.Now}");

        await Task.CompletedTask;
    }
}
```

Performance:

Efficient use of database connections.

Generic:

Can be used with any entity class that has an identifier.

🧩 Requirements

* .NET 6+

🗂️ Package Structure

## Repository Interface (`IRepository<TEntity>`)

This interface provides an abstraction for a generic repository pattern, allowing operations on any entity type. Below is a description of each available method:

### 🔍 Querying Entities
- **`IQueryable<TEntity> Entities`**  
  Gets the entities of the repository. Can be used with AutoMapper's `ProjectTo` for projections.

- **`IQueryable<TEntity> GetAll(FindOptions? findOptions = null)`**  
  Retrieves all entities with optional find options.

- **`IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate, FindOptions? findOptions = null)`**  
  Retrieves all entities that match the specified predicate with optional find options.

- **`IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate, FindOptions? findOptions = null)`**  
  Finds all entities that match the specified predicate with optional find options.

- **`TEntity FindOne(Expression<Func<TEntity, bool>> predicate, FindOptions? findOptions = null)`**  
  Finds a single entity that matches the specified predicate with optional find options.

### ⏳ Asynchronous Queries
- **`Task<IEnumerable<TEntity>> GetAllAsync(int pageNumber, int pageSize)`**  
  Retrieves a paginated list of entities asynchronously.

- **`Task<IEnumerable<TEntity>> GetAllAsync(int pageNumber, int pageSize, params Expression<Func<TEntity, object>>[] includes)`**  
  Retrieves a paginated list of entities asynchronously, with optional includes.

- **`Task<IEnumerable<TEntity>> GetAllAsync(params Expression<Func<TEntity, object>>[] includes)`**  
  Retrieves a list of entities asynchronously, with optional includes.

- **`Task<IEnumerable<TEntity>> GetAllAsync()`**  
  Retrieves all entities asynchronously.

- **`Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate)`**  
  Finds a single entity asynchronously that matches the specified predicate.

### 📌 Adding Entities
- **`Task AddAsync(TEntity entity)`**  
  Adds a single entity to the repository asynchronously.

- **`Task AddAsync(IEnumerable<TEntity> entities)`**  
  Adds multiple entities to the repository asynchronously.

### ✏️ Updating Entities
- **`void UpdateAsync(TEntity entity)`**  
  Updates an existing entity in the repository asynchronously.

### 🗑️ Deleting Entities
- **`void DeleteAsync(TEntity entity)`**  
  Deletes a single entity from the repository asynchronously.

- **`void DeleteAsync(Expression<Func<TEntity, bool>> predicate)`**  
  Deletes entities that match the specified predicate asynchronously.

### 🔢 Utility Methods
- **`bool Any(Expression<Func<TEntity, bool>> predicate)`**  
  Checks if any entities match the specified predicate.

- **`int Count(Expression<Func<TEntity, bool>> predicate)`**  
  Counts the number of entities that match the specified predicate.

---

This repository abstraction helps simplify database operations by providing a structured way to interact with entity data.


🤝 Contribution

Contributions are welcome!

* Fork the repository.
* Create a branch for your feature (git checkout -b feature/NewFeature).
* Commit your changes (git commit -m "Added a new feature X").
* Push to the branch (git push origin feature/NewFeature).
* Open a Pull Request.

⭐ Give it a Star!

If you found this package useful, don't forget to give it a ⭐ on GitHub!
