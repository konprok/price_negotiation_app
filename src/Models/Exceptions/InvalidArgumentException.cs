namespace PriceNegotiationApp.Models.Exceptions;

public sealed class InvalidArgumentException : Exception
{
    public InvalidArgumentException(string? message)
        : base(message)
    {
    }
}