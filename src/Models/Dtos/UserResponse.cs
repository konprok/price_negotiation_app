using PriceNegotiationApp.Database.Entities;

namespace PriceNegotiationApp.Models.Dtos;

public sealed class UserResponse
{
    public UserResponse(UserEntity userEntity)
    {
        Id = userEntity.Id;
        Email = userEntity.Email;
        Name = userEntity.UserName;
    }

    public Guid Id { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
}