using SmartInventorySystemApi.Application.IRepositories;
using SmartInventorySystemApi.Domain.Entities;
using SmartInventorySystemApi.Persistance.Database;

namespace SmartInventorySystemApi.Persistance.Repositories;

public class ItemsHistoryRepository : BaseRepository<ItemHistory>, IItemsHistoryRepository
{
    public ItemsHistoryRepository(MongoDbContext db) : base(db, "ItemsHistory") { }
}
