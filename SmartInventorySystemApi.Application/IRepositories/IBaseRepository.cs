using System.Linq.Expressions;
using MongoDB.Bson;
using SmartInventorySystemApi.Domain.Common;

namespace SmartInventorySystemApi.Application.IRepositories;

public interface IBaseRepository<TEntity> where TEntity : EntityBase
{
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken);

    Task<TEntity> GetOneAsync(ObjectId id, CancellationToken cancellationToken);

    Task<TEntity> GetOneAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken);

    Task<List<TEntity>> GetPageAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);

    Task<List<TEntity>> GetPageAsync(int pageNumber, int pageSize, Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken);

    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken);

    Task<int> GetTotalCountAsync(CancellationToken cancellationToken);

    Task<int> GetCountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken);

    Task<TEntity> DeleteAsync(TEntity entity, CancellationToken cancellationToken);
}
