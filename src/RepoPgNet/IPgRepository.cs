﻿using System.Linq.Expressions;

namespace RepoPgNet;

public interface IPgRepository<TEntity> where TEntity : class
{
    IQueryable<TEntity> GetAllEntities(FindOptions? findOptions = null);
    TEntity FindOne(Expression<Func<TEntity, bool>> predicate, FindOptions? findOptions = null);
    IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate, FindOptions? findOptions = null);
    Task AddAsync(TEntity entity);
    Task AddAsync(IEnumerable<TEntity> entities);
    Task UpdateAsync(TEntity entity);
    Task DeleteAsync(TEntity entity);
    Task DeleteAsync(Expression<Func<TEntity, bool>> predicate);
    bool Any(Expression<Func<TEntity, bool>> predicate);
    int Count(Expression<Func<TEntity, bool>> predicate);
}
