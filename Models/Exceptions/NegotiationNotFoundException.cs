namespace PriceNegotiationApp.Models.Exceptions;

public class NegotiationNotFoundException : Exception
{
    public NegotiationNotFoundException() : base("Unable to find negotiation.") { }
    public NegotiationNotFoundException(string message) : base(message) { }
}