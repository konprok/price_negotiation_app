using PriceNegotiationApp.Models.Dtos;
namespace PriceNegotiationApp.Services.Interfaces;

public interface IUserService
{
    Task<UserResponse> PostUser(UserRegisterDto user);
    Task<UserResponse> GetUser(string userName, string password);
    Task<UserResponse> GetUser(Guid userId);
}