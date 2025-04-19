using System.Linq.Expressions;

namespace Ots.Api.Impl.GenericRepository;

public interface IGenericRepository<TEntity> where TEntity : class
{
    Task SaveChangesAsync();
    Task<TEntity> GetByIdAsync(long id, params string[] includes);
    Task<List<TEntity>> GetAllAsync(params string[] includes);
    Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate, params string[] includes);
    Task<List<TEntity>> Where(Expression<Func<TEntity, bool>> predicate, params string[] includes);
    Task<TEntity> AddAsync(TEntity entity);
    void Update(TEntity entity);
    void Delete(TEntity entity);
    Task DeleteByIdAsync(long id);
}
