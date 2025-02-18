namespace PriceNegotiationApp.Models.Dtos;

public class UserRegisterDto
{
    public UserRegisterDto()
    {
    }

    public UserRegisterDto(string userName, string email, string password)
    {
        UserName = userName;
        Password = password;
        Email = email;
    }

    public string UserName { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
}