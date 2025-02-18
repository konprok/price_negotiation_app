namespace PriceNegotiationApp.Models.Exceptions;

public class InvalidInputException : Exception
{
    public InvalidInputException() : base("One or more input values are invalid") { }
    public InvalidInputException(string message) : base(message) { }
}