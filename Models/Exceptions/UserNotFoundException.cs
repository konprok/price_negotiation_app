namespace PriceNegotiationApp.Models.Exceptions;

public sealed class UserNotFoundException : Exception
{
    public UserNotFoundException() : base("Unable to find user") { }
    public UserNotFoundException(string message) : base(message) { }
}
