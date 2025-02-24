namespace PriceNegotiationApp.Models.Dtos;

public sealed class UserRegisterDto
{
    public UserRegisterDto(string email, string userName, string password)
    {
        Email = email;
        UserName = userName;
        Password = password;
    }

    public string Email { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
}