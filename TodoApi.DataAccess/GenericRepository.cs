using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TodoApi.IDataAccess;

namespace TodoApi.DataAccess;

public class GenericRepository<TEntity>(TodoContext context) : IGenericRepository<TEntity>
    where TEntity : class
{
    private readonly DbSet<TEntity> _entities = context.Set<TEntity>();
    private readonly DbContext _context = context;
    void IGenericRepository<TEntity>.Add(TEntity entity)
    {
        _entities.Add(entity);
        _context.SaveChanges();
    }

    TEntity IGenericRepository<TEntity>.Get(Expression<Func<TEntity, bool>> predicate)
    {
        var entity = _entities.FirstOrDefault(predicate);
        return entity;
    }

    void IGenericRepository<TEntity>.Update(TEntity entity)
    {
        _entities.Update(entity);
        _context.SaveChanges();
    }

    public TEntity Get(Expression<Func<TEntity, bool>> predicate, params string[] includes)
    {
        IQueryable<TEntity> query = _entities;

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        var entity = query.FirstOrDefault(predicate);

        return entity;
    }

    public List<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate, params string[] includes)
    {
        return GetAllInternal(predicate, includes);
    }

    void IGenericRepository<TEntity>.Delete(TEntity entity)
    {
        _entities.Remove(entity);
        _context.SaveChanges();
    }

    private List<TEntity> GetAllInternal(Expression<Func<TEntity, bool>>? predicate, params string[] includes)
    {
        IQueryable<TEntity> query = _context.Set<TEntity>();

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        return query.ToList();
    }
}