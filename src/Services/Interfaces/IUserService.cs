using PriceNegotiationApp.Models.Dtos;

namespace PriceNegotiationApp.Services.Interfaces;

public interface IUserService
{
    Task<UserResponse> PostUser(UserRegisterDto user);
    Task<UserResponse> GetUser(UserLoginDto user);
}