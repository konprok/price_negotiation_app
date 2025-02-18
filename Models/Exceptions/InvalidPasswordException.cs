namespace PriceNegotiationApp.Models.Exceptions;

public class InvalidPasswordException : Exception
{
    public InvalidPasswordException() : base("Invalid password.") { }
    public InvalidPasswordException(string message) : base(message) { }
}