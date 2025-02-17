namespace PriceNegotiationApp.Models.Exceptions;

public sealed class InvalidUserException : Exception
{
    public InvalidUserException() : base("Invalid user.") { }
    public InvalidUserException(string message) : base(message) { }
}