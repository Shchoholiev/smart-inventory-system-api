using MongoDB.Bson;
using SmartInventorySystemApi.Domain.Entities;

namespace SmartInventorySystemApi.Application.IRepositories;

public interface IScanHistoryRepository : IBaseRepository<ScanHistory>
{
    Task<List<ScanHistory>> GetHistoryPageAsync(int page, int size, ObjectId deviceId, CancellationToken cancellationToken);
}
