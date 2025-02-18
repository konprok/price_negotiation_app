namespace PriceNegotiationApp.Models.Exceptions;

public class TimeForNewPropositionHasPassedException : Exception
{
    public TimeForNewPropositionHasPassedException() : base("The time specified for a new proposal has passed") { }
    public TimeForNewPropositionHasPassedException(string message) : base(message) { }
}