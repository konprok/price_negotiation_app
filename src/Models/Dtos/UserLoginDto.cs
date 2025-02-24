namespace PriceNegotiationApp.Models.Dtos;

public sealed class UserLoginDto
{
    public UserLoginDto(string email, string password)
    {
        Email = email;
        Password = password;
    }

    public string Email { get; set; }
    public string Password { get; set; }
}