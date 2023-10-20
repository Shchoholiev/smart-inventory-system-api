using SmartInventorySystemApi.Domain.Common;

namespace SmartInventorySystemApi.Application.Exceptions;

public class EntityNotFoundException : Exception
{
    public EntityNotFoundException(string entityName)
        : base($"\"{entityName}\" was not found.") { }

    public EntityNotFoundException(string message, Exception innerException)
        : base(message, innerException) { }
}
