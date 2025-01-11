using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace RepoPgNet;

public class PgRepository<TEntity> : IPgRepository<TEntity> where TEntity : class
{
    private readonly DbContext _context;

    public PgRepository(DbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(TEntity entity)
    {
        _context.Set<TEntity>().Add(entity);
        await _context.SaveChangesAsync();
    }

    public async Task AddAsync(IEnumerable<TEntity> entities)
    {
        _context.Set<TEntity>().AddRange(entities);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(TEntity entity)
    {
        _context.Set<TEntity>().Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate)
    {
        var entities = Find(predicate);
        _context.Set<TEntity>().RemoveRange(entities);
        await _context.SaveChangesAsync();
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
            Console.WriteLine($"Nenhuma entidade encontrada para o predicado: {predicate}");
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

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
       return await _context.Set<TEntity>().ToListAsync();
    }


    public async Task UpdateAsync(TEntity entity)
    {
        _context.Set<TEntity>().Update(entity);
        await _context.SaveChangesAsync();
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
