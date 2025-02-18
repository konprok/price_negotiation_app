using PriceNegotiationApp.Database.Entities;

namespace PriceNegotiationApp.Models.Dtos;

public sealed class UserResponse
{
    public UserResponse()
    {
    }

    public Guid Id { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }

    public UserResponse(UserEntity userEntity)
    {
        Id = userEntity.Id;
        Name = userEntity.UserName;
        Email = userEntity.Email;
    }
}