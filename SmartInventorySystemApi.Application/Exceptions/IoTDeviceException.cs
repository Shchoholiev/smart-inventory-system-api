namespace SmartInventorySystemApi.Application.Exceptions;

public class IoTDeviceException : Exception
{
    public IoTDeviceException(string message)
        : base(message) { }

    public IoTDeviceException(string message, Exception innerException)
        : base(message, innerException) { }
}

