﻿namespace SmartInventorySystemApi.Application.Exceptions;

public class InvalidPhoneNumberException : Exception
{
    public InvalidPhoneNumberException() { }

    public InvalidPhoneNumberException(string phone) 
        : base($"String {phone} can not be a phone number.") { }
}