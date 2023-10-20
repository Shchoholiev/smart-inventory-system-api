using SmartInventorySystemApi.Application.IRepositories;
using SmartInventorySystemApi.Domain.Entities.Identity;
using SmartInventorySystemApi.Persistance.Database;

namespace SmartInventorySystemApi.Persistance.Repositories;

public class RolesRepository : BaseRepository<Role>, IRolesRepository
{
    public RolesRepository(MongoDbContext db) : base(db, "Roles") { }
}