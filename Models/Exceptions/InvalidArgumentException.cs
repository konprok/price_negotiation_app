namespace PriceNegotiationApp.Models.Exceptions;

public class InvalidArgumentException : Exception
{
    public InvalidArgumentException(string? message)
        : base(message)
    {
    }
}