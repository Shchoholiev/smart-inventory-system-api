using SmartInventorySystemApi.Application.IRepositories;
using SmartInventorySystemApi.Domain.Entities.Identity;
using SmartInventorySystemApi.Persistance.Database;

namespace SmartInventorySystemApi.Persistance.Repositories;

public class RefreshTokensRepository : BaseRepository<RefreshToken>, IRefreshTokensRepository
{
    public RefreshTokensRepository(MongoDbContext db) : base(db, "RefreshTokens") { }
}
