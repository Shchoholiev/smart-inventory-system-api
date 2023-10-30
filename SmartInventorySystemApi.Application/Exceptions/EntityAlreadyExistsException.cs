using SmartInventorySystemApi.Domain.Common;

namespace SmartInventorySystemApi.Application.Exceptions;

public class EntityAlreadyExistsException : Exception 
{
    public EntityAlreadyExistsException(string enitityName)
        : base($"\"{enitityName}\" already exists.") { }

    public EntityAlreadyExistsException(string message, Exception innerException)
        : base(message, innerException) { }

    public EntityAlreadyExistsException(string entityName, string paramName, string paramValue)
        : base($"\"{entityName}\" with {paramName}: \"{paramValue}\" already exists.") { }
}
