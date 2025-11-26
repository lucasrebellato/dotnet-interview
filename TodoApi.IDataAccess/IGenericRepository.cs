using System.Linq.Expressions;

namespace TodoApi.IDataAccess;

public interface IGenericRepository<TEntity>
    where TEntity : class
{
    Task<TEntity> Get(Expression<Func<TEntity, bool>> predicate, params string[] includes);
    Task Add(TEntity entity);
    Task Update(TEntity entity);
    Task Delete(TEntity entity);
    Task<List<TEntity>> GetAll(Expression<Func<TEntity, bool>> predicate, params string[] includes);
    Task SaveChangesAsync();
}
