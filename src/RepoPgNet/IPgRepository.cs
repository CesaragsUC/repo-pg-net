using System.Linq.Expressions;

namespace RepoPgNet;

public interface IPgRepository<TEntity> where TEntity : class
{
    IQueryable<TEntity> GetAll(FindOptions? findOptions = null);
    TEntity FindOne(Expression<Func<TEntity, bool>> predicate, FindOptions? findOptions = null);
    IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate, FindOptions? findOptions = null);
    Task Add(TEntity entity);
    Task Add(IEnumerable<TEntity> entities);
    Task Update(TEntity entity);
    Task Delete(TEntity entity);
    Task Delete(Expression<Func<TEntity, bool>> predicate);
    bool Any(Expression<Func<TEntity, bool>> predicate);
    int Count(Expression<Func<TEntity, bool>> predicate);
}
