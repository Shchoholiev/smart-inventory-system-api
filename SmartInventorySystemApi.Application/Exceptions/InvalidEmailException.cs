namespace SmartInventorySystemApi.Application.Exceptions;

public class InvalidEmailException : Exception
{
    public InvalidEmailException() { }

    public InvalidEmailException(string email) 
        : base($"String {email} can not be an email.") { }
}
