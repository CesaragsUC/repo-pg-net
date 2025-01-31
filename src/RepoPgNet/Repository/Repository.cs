using Microsoft.EntityFrameworkCore;
using RepoPgNet.Abstractions;
using System.Linq.Expressions;

namespace RepoPgNet.Repository;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    private readonly DbContext _context;

    public Repository(DbContext context)
    {
        _context = context;
    }

    public IQueryable<TEntity> Entities => _context.Set<TEntity>();

    public async Task AddAsync(TEntity entity)
    {
        await _context.Set<TEntity>().AddAsync(entity);
    }

    public async Task AddAsync(IEnumerable<TEntity> entities)
    {
        await _context.Set<TEntity>().AddRangeAsync(entities);
    }

    public void DeleteAsync(TEntity entity)
    {
        _context.Set<TEntity>().Remove(entity);
    }

    public void DeleteAsync(Expression<Func<TEntity, bool>> predicate)
    {
        var entities = Find(predicate);
        _context.Set<TEntity>().RemoveRange(entities);
    }

    public TEntity FindOne(Expression<Func<TEntity, bool>> predicate, FindOptions? findOptions = null)
    {
        return Get(findOptions).FirstOrDefault(predicate)!;
    }

    public IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate, FindOptions? findOptions = null)
    {
        return Get(findOptions).Where(predicate);
    }

    public async Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate)
    {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));

        var result = await _context.Set<TEntity>().Where(predicate).FirstOrDefaultAsync();

        if (result == null)
        {
            Console.WriteLine($"No entity was founded with predicated: {predicate}");
        }

        return result!;
    }

    public IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate, FindOptions? findOptions = null)
    {
        return Get(findOptions).Where(predicate);
    }

    public IQueryable<TEntity> GetAll(FindOptions? findOptions = null)
    {
        return Get(findOptions);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(int pageNumber, int pageSize)
    {
        return await _context.Set<TEntity>()
                             .Skip((pageNumber - 1) * pageSize)
                             .Take(pageSize)
                             .ToListAsync();
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(
    int pageNumber,
    int pageSize,
    params Expression<Func<TEntity, object>>[] includes)
    {
        IQueryable<TEntity> query = _context.Set<TEntity>();

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(params Expression<Func<TEntity, object>>[] includes)
    {
        IQueryable<TEntity> query = _context.Set<TEntity>();

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await _context.Set<TEntity>().ToListAsync();
    }


    public void UpdateAsync(TEntity entity)
    {
        _context.Set<TEntity>().Update(entity);
    }

    public bool Any(Expression<Func<TEntity, bool>> predicate)
    {
        return _context.Set<TEntity>().Any(predicate);
    }

    public int Count(Expression<Func<TEntity, bool>> predicate)
    {
        return _context.Set<TEntity>().Count(predicate);
    }

    private DbSet<TEntity> Get(FindOptions? findOptions = null)
    {
        findOptions ??= new FindOptions();
        var entity = _context.Set<TEntity>();
        if (findOptions.IsAsNoTracking && findOptions.IsIgnoreAutoIncludes)
        {
            entity.IgnoreAutoIncludes().AsNoTracking();
        }
        else if (findOptions.IsIgnoreAutoIncludes)
        {
            entity.IgnoreAutoIncludes();
        }
        else if (findOptions.IsAsNoTracking)
        {
            entity.AsNoTracking();
        }
        return entity;
    }
}
