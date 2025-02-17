namespace PriceNegotiationApp.Models.Exceptions;

public sealed class UserAlreadyExistException : Exception
{
    public UserAlreadyExistException() : base("User with this name or email already exists") { }
    public UserAlreadyExistException(string message) : base(message) { }
}