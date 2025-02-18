namespace PriceNegotiationApp.Models.Exceptions;

public class NegotiationDoesNotExistException : Exception
{
    public NegotiationDoesNotExistException() : base("User did not start the negotiation about this product yet.") { }
    public NegotiationDoesNotExistException(string message) : base(message) { }
}