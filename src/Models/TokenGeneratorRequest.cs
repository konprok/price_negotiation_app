namespace PriceNegotiationApp.Models;

public class TokenGeneratorRequest
{
    public Guid UserId { get; set; }
    
    public string Email { get; set; }

    public Dictionary<string, object> CustomClaims { get; set; } = new();
}