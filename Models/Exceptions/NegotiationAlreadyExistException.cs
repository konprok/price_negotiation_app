namespace PriceNegotiationApp.Models.Exceptions;

public class NegotiationAlreadyExistException : Exception
{
    public NegotiationAlreadyExistException() : base("The user is already negotiating for this product.") { }
    public NegotiationAlreadyExistException(string message) : base(message) { }
}