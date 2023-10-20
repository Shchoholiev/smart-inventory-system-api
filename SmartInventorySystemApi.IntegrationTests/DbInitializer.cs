using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using SmartInventorySystemApi.Domain.Entities.Identity;
using SmartInventorySystemApi.Infrastructure.Services.Identity;
using SmartInventorySystemApi.Persistance.Database;

namespace SmartInventorySystemApi.IntegrationTests;

public class DbInitializer
{
    private readonly MongoDbContext _dbContext;

    public DbInitializer(MongoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void InitializeDb()
    {
        _dbContext.Client.DropDatabase(_dbContext.Db.DatabaseNamespace.DatabaseName);
        
        InitializeUsersAsync().Wait();
    }

    public async Task InitializeUsersAsync()
    {
        #region Roles

        var rolesCollection = _dbContext.Db.GetCollection<Role>("Roles");

        var userRole = new Role
        {
            Name = "User"
        };
        await rolesCollection.InsertOneAsync(userRole);

        var ownerRole = new Role
        {
            Name = "Owner"
        };
        await rolesCollection.InsertOneAsync(ownerRole);

        var adminRole = new Role
        {
            Name = "Admin"
        };
        await rolesCollection.InsertOneAsync(adminRole);

        #endregion

        #region Users

        var passwordHasher = new PasswordHasher(new Logger<PasswordHasher>(new LoggerFactory()));
        var usersCollection = _dbContext.Db.GetCollection<User>("Users");

        var testUser = new User
        {
            Id = ObjectId.Parse("652c3b89ae02a3135d6409fc"),
            Email = "test@gmail.com",
            Phone = "+380123456789",
            Roles = new List<Role> { userRole },
            PasswordHash = passwordHasher.Hash("Yuiop12345"),
            CreatedById = ObjectId.Empty,
            CreatedDateUtc = DateTime.UtcNow
        };
        await usersCollection.InsertOneAsync(testUser);

        var adminUser = new User
        {
            Id = ObjectId.Parse("652c3b89ae02a3135d6408fc"),
            Email = "admin@gmail.com",
            Phone = "+12345678901",
            Roles = new List<Role> { userRole, adminRole },
            PasswordHash = passwordHasher.Hash("Yuiop12345"),
            CreatedById = ObjectId.Empty,
            CreatedDateUtc = DateTime.UtcNow
        };
        await usersCollection.InsertOneAsync(adminUser);

        #endregion

        #region RefreshTokens

        var refreshTokensCollection = _dbContext.Db.GetCollection<RefreshToken>("RefreshTokens");

        var refreshToken = new RefreshToken
        {
            Token = "test-refresh-token",
            ExpiryDateUTC = DateTime.UtcNow.AddDays(-7),
            CreatedById = testUser.Id,
            CreatedDateUtc = DateTime.UtcNow
        };
        await refreshTokensCollection.InsertOneAsync(refreshToken);

        #endregion
    }
}
