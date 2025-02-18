namespace PriceNegotiationApp.Models.Exceptions;

public class NegotiationHasEndedException : Exception
{
    public NegotiationHasEndedException() : base("This negotiation has ended.") { }
    public NegotiationHasEndedException(string message) : base(message) { }
}