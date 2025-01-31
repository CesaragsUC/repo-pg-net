using Microsoft.EntityFrameworkCore;

namespace RepoPgNet.Abstractions;

public interface IUnitOfWork<TContext> : IDisposable where TContext : DbContext
{
    IRepository<TEntity> Repository<TEntity>() where TEntity : class, new();
    Task<bool> Commit();
    Task Rollback();
}