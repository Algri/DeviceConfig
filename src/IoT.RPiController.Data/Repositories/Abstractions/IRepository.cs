using System.Linq.Expressions;

namespace IoT.RPiController.Data.Repositories.Abstractions
{
    public interface IRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> AsQueryable();
        Task<List<TEntity>> GetAllAsync();
        ValueTask<TEntity?> GetByIdAsync(object id);
        Task AddAsync(TEntity entity);
        Task AddRangeAsync(IEnumerable<TEntity> entities);
        Task AddIfNotExistAsync(Expression<Func<TEntity, object>> identifierExpression, params TEntity[] entities);
        void Delete(TEntity entity);
        Task DeleteAsync(object id);
        Task DeleteRange(IEnumerable<object> ids);
        void Detach(TEntity entity);
        void Update(TEntity entity);
        void UpdateRange(IEnumerable<TEntity> entities);
        void UpdateSingleColumn<TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> func);
        IEnumerable<TProperty> PluckColumn<TProperty>(Expression<Func<TEntity, TProperty>> columnExpression);
        Task SaveChangesAsync();
    }
}