using PriceNegotiationApp.Models.Dtos;

namespace PriceNegotiationApp.Database.Entities;

public sealed class UserEntity
{
    public UserEntity()
    {
    }

    public UserEntity(UserRegisterDto userRegisterDto)
    {
        Email = userRegisterDto.Email;
        UserName = userRegisterDto.UserName;
        PasswordHash = userRegisterDto.Password;
    }

    public Guid Id { get; set; }
    public string Email { get; set; }
    public string UserName { get; set; }
    public string PasswordHash { get; set; }
}