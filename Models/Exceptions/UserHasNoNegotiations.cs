namespace PriceNegotiationApp.Models.Exceptions;

public class UserHasNoNegotiations : Exception
{
    public UserHasNoNegotiations() : base("User has currently no negotiations.") { }
    public UserHasNoNegotiations(string message) : base(message) { }
}