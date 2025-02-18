namespace PriceNegotiationApp.Models.Exceptions;

public class PropositionUnderConsiderationException : Exception
{
    public PropositionUnderConsiderationException() : base("Proposition is still under consideration") { }
    public PropositionUnderConsiderationException(string message) : base(message) { }
}