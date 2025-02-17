namespace PriceNegotiationApp.Models.Dtos;

public class UserRegisterDto(string userName, string email, string password)
{
    public string UserName { get; set; } = userName;
    public string Password { get; set; } = password;
    public string Email { get; set; } = email;
}