using System.Linq.Expressions;

namespace TodoApi.IDataAccess;

public interface IGenericRepository<TEntity>
    where TEntity : class
{
    void Add(TEntity entity);
    TEntity Get(Expression<Func<TEntity, bool>> predicate);
    void Update(TEntity entity);
    void Delete(TEntity entity);
    TEntity Get(Expression<Func<TEntity, bool>> predicate, params string[] includes);
    public List<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate, params string[] includes);
}
