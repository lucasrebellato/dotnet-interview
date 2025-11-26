using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TodoApi.Domain.Interfaces;
using TodoApi.IDataAccess;

namespace TodoApi.DataAccess;

public class GenericRepository<TEntity>(TodoContext context) : IGenericRepository<TEntity>
    where TEntity : class
{
    private readonly DbSet<TEntity> _entities = context.Set<TEntity>();
    private readonly DbContext _context = context;
    async Task IGenericRepository<TEntity>.Add(TEntity entity)
    {
        await _entities.AddAsync(entity);
        _context.SaveChanges();
    }

    async Task<TEntity> IGenericRepository<TEntity>.Get(Expression<Func<TEntity, bool>> predicate, params string[] includes)
    {
        IQueryable<TEntity> query = _entities;

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        var entity = await query.FirstOrDefaultAsync(predicate);

        return entity;
    }

    async Task IGenericRepository<TEntity>.Update(TEntity entity)
    {
        _entities.Update(entity);
        await _context.SaveChangesAsync();
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

    async Task<List<TEntity>> IGenericRepository<TEntity>.GetAll(Expression<Func<TEntity, bool>> predicate, params string[] includes)
    {
        return (await GetAllInternal(predicate, includes));
    }

    async Task IGenericRepository<TEntity>.Delete(TEntity entity)
    {
        if (entity is ISoftDeletable softEntity)
        {
            softEntity.IsDeleted = true;
            _entities.Update(entity);
        }
        else
        {
            _entities.Remove(entity);
        }

        await _context.SaveChangesAsync();
    }

    private async Task<List<TEntity>> GetAllInternal(Expression<Func<TEntity, bool>>? predicate, params string[] includes)
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

        return (await query.ToListAsync());
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}