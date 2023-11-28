using MongoDB.Bson;
using MongoDB.Driver;
using SmartInventorySystemApi.Application.IRepositories;
using SmartInventorySystemApi.Domain.Entities;
using SmartInventorySystemApi.Persistance.Database;

namespace SmartInventorySystemApi.Persistance.Repositories;

public class ScanHistoryRepository : BaseRepository<ScanHistory>, IScanHistoryRepository
{
    public ScanHistoryRepository(MongoDbContext db) : base(db, "ScanHistory") { }

    public async Task<List<ScanHistory>> GetHistoryPageAsync(int page, int size, ObjectId deviceId, CancellationToken cancellationToken)
    {
        return await this._collection
            .Find(h => h.DeviceId == deviceId)
            .SortByDescending(h => h.CreatedDateUtc)
            .Skip((page - 1) * size)
            .Limit(size)
            .ToListAsync(cancellationToken);
    }
}