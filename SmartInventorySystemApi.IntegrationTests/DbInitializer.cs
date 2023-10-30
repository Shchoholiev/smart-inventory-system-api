using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using SmartInventorySystemApi.Domain.Entities;
using SmartInventorySystemApi.Domain.Entities.Identity;
using SmartInventorySystemApi.Domain.Enums;
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
        InitializeGroupsAsync().Wait();
        InitializeDevicesAsync().Wait();
        InitializeShelvesAsync().Wait();
        InitializeItemsAsync().Wait();
        InitializeScansHistoryAsync().Wait();
        InitializeItemsHistoryAsync().Wait();
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

        var updateTestUser = new User
        {
            Id = ObjectId.Parse("652c3b89ae02a3135d6309fc"),
            Email = "update@gmail.com",
            Phone = "+380123446789",
            Roles = new List<Role> { userRole },
            PasswordHash = passwordHasher.Hash("Yuiop12345"),
            CreatedById = ObjectId.Empty,
            CreatedDateUtc = DateTime.UtcNow
        };
        await usersCollection.InsertOneAsync(updateTestUser);

        var groupOwner = new User
        {
            Id = ObjectId.Parse("652c3b89ae02a3135d6419fc"),
            Email = "owner@gmail.com",
            Phone = "+380123456689",
            Roles = new List<Role> { userRole, ownerRole },
            GroupId = ObjectId.Parse("652c3b89ae02a3135d6429fc"), // see group creation below
            PasswordHash = passwordHasher.Hash("Yuiop12345"),
            CreatedById = ObjectId.Empty,
            CreatedDateUtc = DateTime.UtcNow
        };
        await usersCollection.InsertOneAsync(groupOwner);

        var groupUser = new User
        {
            Id = ObjectId.Parse("652c3b89ae02a3135d6439fc"),
            Email = "group@gmail.com",
            Phone = "+380123456889",
            Roles = new List<Role> { userRole },
            GroupId = ObjectId.Parse("652c3b89ae02a3135d6429fc"), // see group creation below
            PasswordHash = passwordHasher.Hash("Yuiop12345"),
            CreatedById = ObjectId.Empty,
            CreatedDateUtc = DateTime.UtcNow
        };
        await usersCollection.InsertOneAsync(groupUser);

        var groupUser2 = new User
        {
            Id = ObjectId.Parse("652c3b89ae02a3135d6432fc"),
            Email = "group2@gmail.com",
            Phone = "+380123456779",
            Roles = new List<Role> { userRole },
            GroupId = ObjectId.Parse("652c3b89ae02a3135d6429fc"), // see group creation below
            PasswordHash = passwordHasher.Hash("Yuiop12345"),
            CreatedById = ObjectId.Empty,
            CreatedDateUtc = DateTime.UtcNow
        };
        await usersCollection.InsertOneAsync(groupUser2);

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

    public async Task InitializeGroupsAsync()
    {
        var groupsCollection = _dbContext.Db.GetCollection<Group>("Groups");

        var group = new Group
        {
            Id = ObjectId.Parse("652c3b89ae02a3135d6429fc"),
            Name = "Test Group 1",
            CreatedById = ObjectId.Parse("652c3b89ae02a3135d6419fc"),
            CreatedDateUtc = DateTime.UtcNow
        };
        await groupsCollection.InsertOneAsync(group);

        var secondGroup = new Group
        {
            Id = ObjectId.Parse("662c3b89ae02a3135d6429fc"),
            Name = "Test Group 1",
            CreatedById = ObjectId.Empty,
            CreatedDateUtc = DateTime.UtcNow
        };
        await groupsCollection.InsertOneAsync(secondGroup);
    }

    public async Task InitializeDevicesAsync()
    {
        var devicesCollection = _dbContext.Db.GetCollection<Device>("Devices");

        var device = new Device
        {
            Id = ObjectId.Parse("651c3b89ae02a3135d6439fc"),
            Name = "Test Device 1",
            Type = DeviceType.Rack4ShelfController,
            Guid = Guid.Parse("7a78a8b2-6cf6-427d-8ed2-a5e117d8fd3f"), 
            GroupId = ObjectId.Parse("652c3b89ae02a3135d6429fc"), // See above
            CreatedById = ObjectId.Parse("652c3b89ae02a3135d6408fc"), // See above (admin@gmail.com)
            CreatedDateUtc = DateTime.UtcNow
        };
        await devicesCollection.InsertOneAsync(device);

        var updateDevice = new Device
        {
            Id = ObjectId.Parse("653c3b89ae02a3135d6439fc"),
            Name = "Test Device for Update",
            Type = DeviceType.Rack4ShelfController,
            CreatedById = ObjectId.Parse("652c3b89ae02a3135d6408fc"), // See above (admin@gmail.com)
            CreatedDateUtc = DateTime.UtcNow
        };
        await devicesCollection.InsertOneAsync(updateDevice);

        var accessPointevice = new Device
        {
            Id = ObjectId.Parse("753c3b89ae02a3135d6139fc"),
            Name = "Access Point Device",
            Type = DeviceType.AccessPoint,
            Guid = Guid.Parse("4d09b6ae-7675-4603-b632-9e834de6957f"), 
            CreatedById = ObjectId.Parse("652c3b89ae02a3135d6408fc"), // See above (admin@gmail.com)
            CreatedDateUtc = DateTime.UtcNow
        };
        await devicesCollection.InsertOneAsync(accessPointevice);
    }

    public async Task InitializeShelvesAsync()
    {
        var shelvesCollection = _dbContext.Db.GetCollection<Shelf>("Shelves");

        var shelf1 = new Shelf
        {
            Id = ObjectId.Parse("651c1b09ae02a3135d6439fc"),
            Name = "Test Device 1 Shelf 1",
            PositionInRack = 1,
            GroupId = ObjectId.Parse("652c3b89ae02a3135d6429fc"), // See above
            DeviceId = ObjectId.Parse("651c3b89ae02a3135d6439fc"), // See above
            CreatedById = ObjectId.Parse("652c3b89ae02a3135d6408fc"), // See above
            CreatedDateUtc = DateTime.UtcNow
        };

        var shelf2 = new Shelf
        {
            Id = ObjectId.Parse("651c2b09ae02a3135d6439fc"),
            Name = "Test Device 1 Shelf 2",
            PositionInRack = 2,
            GroupId = ObjectId.Parse("652c3b89ae02a3135d6429fc"), // See above
            DeviceId = ObjectId.Parse("651c3b89ae02a3135d6439fc"), // See above
            CreatedById = ObjectId.Parse("652c3b89ae02a3135d6408fc"), // See above
            CreatedDateUtc = DateTime.UtcNow
        };

        var shelf3 = new Shelf
        {
            Id = ObjectId.Parse("651c3b09ae02a3135d6439fc"),
            Name = "Test Device 1 Shelf 3",
            PositionInRack = 3,
            GroupId = ObjectId.Parse("652c3b89ae02a3135d6429fc"), // See above
            DeviceId = ObjectId.Parse("651c3b89ae02a3135d6439fc"), // See above
            CreatedById = ObjectId.Parse("652c3b89ae02a3135d6408fc"), // See above
            CreatedDateUtc = DateTime.UtcNow
        };

        var shelf4 = new Shelf
        {
            Id = ObjectId.Parse("651c4b89ae02a3135d6439fc"),
            Name = "Test Device 1 Shelf 4",
            PositionInRack = 4,
            GroupId = ObjectId.Parse("652c3b89ae02a3135d6429fc"), // See above
            DeviceId = ObjectId.Parse("651c3b89ae02a3135d6439fc"), // See above
            IsLitUp = true,
            CreatedById = ObjectId.Parse("652c3b89ae02a3135d6408fc"), // See above
            CreatedDateUtc = DateTime.UtcNow
        };

        await shelvesCollection.InsertManyAsync(new List <Shelf> { shelf1, shelf2, shelf3, shelf4 });
    }

    public async Task InitializeItemsAsync()
    {
        var itemsCollection = _dbContext.Db.GetCollection<Item>("Items");

        var item1 = new Item
        {
            Id = ObjectId.Parse("651c1b01ae02a3135d6439fc"),
            Name = "Test Item 1",
            Description = "This is a test item 1",
            IsTaken = false,
            ShelfId = ObjectId.Parse("651c1b09ae02a3135d6439fc"), // See above
            GroupId = ObjectId.Parse("652c3b89ae02a3135d6429fc"), // See above
            CreatedById = ObjectId.Parse("652c3b89ae02a3135d6408fc"), // See above
            CreatedDateUtc = DateTime.UtcNow
        };

        var item2 = new Item
        {
            Id = ObjectId.Parse("651c1b02ae02a3135d6439fc"),
            Name = "Test Item 2",
            Description = "This is a test item 2",
            IsTaken = true,
            ShelfId = ObjectId.Parse("651c1b09ae02a3135d6439fc"), // See above
            GroupId = ObjectId.Parse("652c3b89ae02a3135d6429fc"), // See above
            CreatedById = ObjectId.Parse("652c3b89ae02a3135d6408fc"), // See above
            CreatedDateUtc = DateTime.UtcNow
        };

        var item3 = new Item
        {
            Id = ObjectId.Parse("651c1b03ae02a3135d6439fc"),
            Name = "Test Item 3",
            Description = "This is a test item 3",
            IsTaken = false,
            ShelfId = ObjectId.Parse("651c1b09ae02a3135d6439fc"), // See above
            GroupId = ObjectId.Parse("652c3b89ae02a3135d6429fc"), // See above
            CreatedById = ObjectId.Parse("652c3b89ae02a3135d6408fc"), // See above
            CreatedDateUtc = DateTime.UtcNow
        };

        var item4 = new Item
        {
            Id = ObjectId.Parse("651c1b04ae02a3135d6439fc"),
            Name = "Test Item 4",
            Description = "This is a test item 4",
            IsTaken = false,
            ShelfId = ObjectId.Parse("651c1b09ae02a3135d6439fc"), // See above
            GroupId = ObjectId.Parse("652c3b89ae02a3135d6429fc"), // See above
            CreatedById = ObjectId.Parse("652c3b89ae02a3135d6408fc"), // See above
            CreatedDateUtc = DateTime.UtcNow
        };

        var charger = new Item
        {
            Id = ObjectId.Parse("651c1b04ae02a8135d6439fc"),
            Name = "Magsafe Charger",
            Description = "Apple Magsafe Charger",
            IsTaken = false,
            ShelfId = ObjectId.Parse("651c1b09ae02a3135d6439fc"), // See above
            GroupId = ObjectId.Parse("652c3b89ae02a3135d6429fc"), // See above
            CreatedById = ObjectId.Parse("652c3b89ae02a3135d6408fc"), // See above
            CreatedDateUtc = DateTime.UtcNow
        };

        var movementTestItem = new Item
        {
            Id = ObjectId.Parse("651c1b04ae92a8135d6439fc"),
            Name = "Movement Test",
            Description = "Test",
            IsTaken = true,
            ShelfId = ObjectId.Parse("651c4b89ae02a3135d6439fc"), // See above
            GroupId = ObjectId.Parse("652c3b89ae02a3135d6429fc"), // See above
            CreatedById = ObjectId.Parse("652c3b89ae02a3135d6408fc"), // See above
            CreatedDateUtc = DateTime.UtcNow
        };

        await itemsCollection.InsertManyAsync(new List<Item> { item1, item2, item3, item4, charger, movementTestItem });
    }

    public async Task InitializeScansHistoryAsync()
    {
        var historyCollection = _dbContext.Db.GetCollection<ScanHistory>("ScanHistory");

        var scanHistory1 = new ScanHistory
        {
            Id = ObjectId.Parse("651c1a01ae02a1135d6439fc"),
            DeviceId = ObjectId.Parse("753c3b89ae02a3135d6139fc"), // See above
            ScanType = ScanType.QRCode,
            Result = "Item Found",
            CreatedById = ObjectId.Empty, // Auth for Access Point Device is not implemented yet
            CreatedDateUtc = DateTime.UtcNow
        };

        var scanHistory2 = new ScanHistory
        {
            Id = ObjectId.Parse("651c1a01ae02a1135d6431fc"),
            DeviceId = ObjectId.Parse("753c3b89ae02a3135d6139fc"), // See above
            ScanType = ScanType.QRCode,
            Result = "Item Not Found",
            CreatedById = ObjectId.Empty, // Auth for Access Point Device is not implemented yet
            CreatedDateUtc = DateTime.UtcNow
        };

        var scanHistory3 = new ScanHistory
        {
            Id = ObjectId.Parse("651c1a01ae02a1135d6432fc"),
            DeviceId = ObjectId.Parse("753c3b89ae02a3135d6139fc"), // See above
            ScanType = ScanType.QRCode,
            Result = "Item Found",
            CreatedById = ObjectId.Empty, // Auth for Access Point Device is not implemented yet
            CreatedDateUtc = DateTime.UtcNow
        };
        await historyCollection.InsertManyAsync(new List<ScanHistory> { scanHistory1, scanHistory2, scanHistory3 });
    }

    public async Task InitializeItemsHistoryAsync()
    {
        var historyCollection = _dbContext.Db.GetCollection<ItemHistory>("ItemHistory");

        var itemHistory1 = new ItemHistory
        {
            Id = ObjectId.Parse("651c1a01ae02a1125d6439fc"),
            ItemId = ObjectId.Parse("651c1b04ae92a8135d6439fc"), // See above
            Comment = "Light Turned on By AccessPointDevice. QRCode Scan.",
            IsTaken = true,
            Type = ItemHistoryType.Scan,
            CreatedById = ObjectId.Parse("652c3b89ae02a3135d6409fc"), // See above
            CreatedDateUtc = DateTime.UtcNow
        };
        await historyCollection.InsertManyAsync(new List<ItemHistory> { itemHistory1 });
    }
}
