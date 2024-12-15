using IoT.RPiController.Data.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace IoT.RPiController.Data.Repositories.Implementations;

public class Repository<TEntity>(RPiContext context) : IRepository<TEntity>
    where TEntity : class
{
    protected readonly DbSet<TEntity> _dbSet = context.Set<TEntity>();

    public IQueryable<TEntity> AsQueryable()
    {
        return _dbSet;
    }

    public async Task<List<TEntity>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async ValueTask<TEntity?> GetByIdAsync(object id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task AddAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public async Task AddRangeAsync(IEnumerable<TEntity> entities)
    {
        await _dbSet.AddRangeAsync(entities);
    }

    public async Task AddIfNotExistAsync(Expression<Func<TEntity, object>> identifierExpression, params TEntity[] entities)
    {
        var pi = GetPropertyInfo(identifierExpression);
        var parameter = Expression.Parameter(typeof(TEntity));

        foreach (var entity in entities)
        {
            var id = pi.GetValue(entity, null);
            var body = Expression.Equal(Expression.Property(parameter, pi), Expression.Constant(id));
            var found = await _dbSet.SingleOrDefaultAsync(Expression.Lambda<Func<TEntity, bool>>(body, new[] { parameter }));

            if (found == null)
            {
                await _dbSet.AddAsync(entity);
            }
        }
    }

    public void Update(TEntity entity)
    {
        if (context.Entry(entity).State != EntityState.Detached) return;

        _dbSet.Attach(entity);

        // we need to mark as modified only entities that not in our context, other changes will be tracked by ChangesTracker!
        context.Entry(entity).State = EntityState.Modified;
    }

    public void UpdateRange(IEnumerable<TEntity> entities)
    {
        _dbSet.UpdateRange(entities);
    }

    public void Delete(TEntity entity)
    {
        if (context.Entry(entity).State == EntityState.Detached)
        {
            _dbSet.Attach(entity);
        }

        _dbSet.Remove(entity);
    }

    public async Task DeleteAsync(object id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
        }
    }

    public async Task DeleteRange(IEnumerable<object> ids)
    {
        foreach (var id in ids)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }
    }

    public void Detach(TEntity entity)
    {
        context.Entry(entity).State = EntityState.Detached;
    }

    public IQueryable<T> Include<T>(Expression<Func<T, object>> criteria) where T : class
    {
        return context.Set<T>().Include(criteria);
    }

    public void UpdateSingleColumn<TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> func)
    {
        if (context.Entry(entity).State != EntityState.Detached) return;

        _dbSet.Attach(entity);
        context.Entry(entity).Property(func).IsModified = true;
    }

    private static PropertyInfo GetPropertyInfo<TSource, TProperty>(Expression<Func<TSource, TProperty>> expression)
    {
        var type = typeof(TSource);
        var member = expression.Body as MemberExpression;

        if (member == null)
            throw new ArgumentException(
                $"Expression '{expression.ToString()}' refers to a method, not a property.");

        var propInfo = member.Member as PropertyInfo;
        if (propInfo == null)
            throw new ArgumentException($"Expression '{expression.ToString()}' refers to a field, not a property.");

        if (propInfo.ReflectedType != null && !propInfo.ReflectedType.IsAssignableFrom(type))
            throw new ArgumentException(
                $"Expression '{expression.ToString()}' refers to a property that is not from type {type}.");

        return propInfo;
    }

    public IEnumerable<TProperty> PluckColumn<TProperty>(Expression<Func<TEntity, TProperty>> columnExpression)
    {
        return AsQueryable().Select(columnExpression).ToList();
    }

    public async Task SaveChangesAsync()
    {
        try
        {
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}