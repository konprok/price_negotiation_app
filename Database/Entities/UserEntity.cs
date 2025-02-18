namespace PriceNegotiationApp.Database.Entities;

public sealed class UserEntity
{
    public  Guid Id { get; set; }
    public required string UserName { get; set; }
    public required string PasswordHash { get; set; }
    public required string Email { get; set; }
}