namespace PriceNegotiationApp.Models.Exceptions;

public class ErrorMessages
{
    public const string UserNotFound = "Unable to find user";
    public const string UserAlreadyExist = "User with this name or email already exists.";
    public const string InvalidUser = "Invalid user.";
    
    public const string InvalidInput = "One or more input values are invalid.";
    public const string InvalidEmail = "Wrong email format.";
    public const string InvalidPassword = "Invalid password.";
    
    public const string TimeForNewPropositionHasPassed = "The time specified for a new proposal has passed.";
    public const string PropositionUnderConsideration = "Proposition is still under consideration.";
    public const string PropositionsLimitReached = "Client has reached limit of propositions in this negotiation.";
    
    public const string ProductNotFoundException = "Product not found.";
    
    public const string UserHasNoNegotiations = "User has currently no negotiations.";
    public const string NegotiationNotFound = "Unable to find negotiation.";
    public const string NegotiationHasEnded = "This negotiation has ended.";
    public const string NegotiationDoesNotExist = "User did not start the negotiation about this product yet.";
}