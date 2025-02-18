namespace PriceNegotiationApp.Models.Exceptions;

public class PropositionsLimitReachedException : Exception
{
    public PropositionsLimitReachedException() : base("User has reached limit of propositions in this negotiation.") { }
    public PropositionsLimitReachedException(string message) : base(message) { }
}