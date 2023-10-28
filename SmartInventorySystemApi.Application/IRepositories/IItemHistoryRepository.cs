using MongoDB.Bson;
using SmartInventorySystemApi.Domain.Entities;

namespace SmartInventorySystemApi.Application.IRepositories;

public interface IItemHistoryRepository : IBaseRepository<ItemHistory>
{
    Task<List<ItemHistory>> GetHistoryPageAsync(int page, int size, ObjectId itemId, CancellationToken cancellationToken);
}
