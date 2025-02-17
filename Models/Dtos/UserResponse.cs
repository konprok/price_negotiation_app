using PriceNegotiationApp.Database.Entities;

namespace PriceNegotiationApp.Models.Dtos;

public sealed class UserResponse(UserEntity userEntity)
{
    public Guid Id {get; set;} = userEntity.Id;
    public string Email { get; set; } = userEntity.Email;
    public string Name { get; set; } = userEntity.UserName;
}