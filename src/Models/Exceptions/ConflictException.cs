namespace PriceNegotiationApp.Models.Exceptions;

public sealed class ConflictException : Exception
{
    public ConflictException(string? message)
        : base(message)
    {
    }
}