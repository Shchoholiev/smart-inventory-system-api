namespace SmartInventorySystemApi.Domain.Enums;

public enum DeviceType
{
    // TODO: expect string in request and convert to enum
    Unknown, // To enforce API users to set type explicitly
    Rack4ShelfController, // Controls a rack of 4 shelves
    AccessPoint
}
