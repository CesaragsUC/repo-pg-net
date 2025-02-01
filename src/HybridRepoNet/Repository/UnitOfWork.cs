using Microsoft.EntityFrameworkCore;
using HybridRepoNet.Abstractions;
using System.Collections;

namespace HybridRepoNet.Repository;

public class UnitOfWork<TContext> : IUnitOfWork<TContext> where TContext : DbContext
{
    private readonly TContext _dbContext;
    private Hashtable _repositories;
    private bool _disposed;
    private readonly IDomainEvent _dispatcher;
    public UnitOfWork(TContext dbContext, IDomainEvent dispatcher)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _dispatcher = dispatcher;
    }

    public IRepository<TEntity> Repository<TEntity>() where TEntity : class, new()
    {
        if (_repositories is null)
            _repositories = new Hashtable();

        var type = typeof(TEntity).Name;

        if (!_repositories.ContainsKey(type))
        {
            var repositoryType = typeof(Repository<>);
            var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TEntity)), _dbContext);

            _repositories.Add(type, repositoryInstance);
        }

        return (IRepository<TEntity>)_repositories[type];
    }

    public Task Rollback()
    {
        _dbContext.ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
        return Task.CompletedTask;
    }

    public async Task<bool> Commit()
    {
        bool result = await _dbContext.SaveChangesAsync() > 0;

        if (_dispatcher == null) return result;

        var entitiesWithEvents = _dbContext.ChangeTracker.Entries<BaseEntity>()
        .Select(e => e.Entity)
        .Where(e => e.DomainEvents.Any())
        .ToArray();

        await _dispatcher.DispatchAndClearEvents(entitiesWithEvents);

        return result;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            if (disposing)
            {
                _dbContext.Dispose();
            }
        }
        _disposed = true;
    }
}
