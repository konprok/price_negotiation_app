namespace PriceNegotiationApp.Models.Dtos;

public class UserLoginDto(string password, string email)
{
    public string Password { get; set; } = password;
    public string Email { get; set; } = email;
}