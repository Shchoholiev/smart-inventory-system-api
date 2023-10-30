using MongoDB.Bson;

namespace SmartInventorySystemApi.Infrastructure.Services.Identity;

public abstract class ServiceBase
{
    public ObjectId ParseObjectId(string? id)
    {
        if (ObjectId.TryParse(id, out ObjectId objectId))
        {
            return objectId;
        }
        
        throw new InvalidDataException("Provided id cannot be parsed to a MongoDb ObjectId.");
    }
}
