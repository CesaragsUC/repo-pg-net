using RepoPgNet.Repository;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RepoPgNet.Abstractions;

public interface IRepository<TEntity> where TEntity : class
{
    /// <summary>
    /// Gets the entities of the repository. Can use with Automapper ProjectTo
    /// </summary>
    IQueryable<TEntity> Entities { get; }

    /// <summary>
    /// Retrieves all entities with optional find options.
    /// </summary>
    IQueryable<TEntity> GetAll(FindOptions? findOptions = null);

    /// <summary>
    /// Retrieves all entities that match the specified predicate with optional find options.
    /// </summary>
    IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate, FindOptions? findOptions = null);

    /// <summary>
    /// Asynchronously retrieves a paginated list of entities.
    /// </summary>
    Task<IEnumerable<TEntity>> GetAllAsync(int pageNumber, int pageSize);

    /// <summary>
    /// Asynchronously retrieves a paginated list of entities with optional includes.
    /// </summary>
    Task<IEnumerable<TEntity>> GetAllAsync(
        int pageNumber,
        int pageSize,
        params Expression<Func<TEntity, object>>[] includes);


    /// <summary>
    /// Asynchronously retrieves a list of entities with optional includes.
    /// </summary>
    Task<IEnumerable<TEntity>> GetAllAsync(params Expression<Func<TEntity, object>>[] includes);

    /// <summary>
    /// Asynchronously retrieves all entities.
    /// </summary>
    Task<IEnumerable<TEntity>> GetAllAsync();

    /// <summary>
    /// Finds a single entity that matches the specified predicate with optional find options.
    /// </summary>
    TEntity FindOne(Expression<Func<TEntity, bool>> predicate, FindOptions? findOptions = null);

    /// <summary>
    /// Finds all entities that match the specified predicate with optional find options.
    /// </summary>
    IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate, FindOptions? findOptions = null);

    /// <summary>
    /// Asynchronously adds a single entity to the repository.
    /// </summary>
    Task AddAsync(TEntity entity);

    /// <summary>
    /// Asynchronously adds multiple entities to the repository.
    /// </summary>
    Task AddAsync(IEnumerable<TEntity> entities);

    /// <summary>
    /// Asynchronously updates an existing entity in the repository.
    /// </summary>
    void UpdateAsync(TEntity entity);

    /// <summary>
    /// Asynchronously deletes a single entity from the repository.
    /// </summary>
    void DeleteAsync(TEntity entity);

    /// <summary>
    /// Asynchronously deletes entities that match the specified predicate from the repository.
    /// </summary>
    void DeleteAsync(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// Checks if any entities match the specified predicate.
    /// </summary>
    bool Any(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// Counts the number of entities that match the specified predicate.
    /// </summary>
    int Count(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// Asynchronously finds a single entity that matches the specified predicate.
    /// </summary>
    Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate);
}
