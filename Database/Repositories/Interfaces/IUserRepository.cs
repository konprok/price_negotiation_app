using PriceNegotiationApp.Database.Entities;
using PriceNegotiationApp.Models.Dtos;

namespace PriceNegotiationApp.Database.Repositories.Interfaces;

public interface IUserRepository
{
    Task<UserEntity?> GetUser(string userEmail);
    Task<UserEntity?> GetUser(Guid userId);
    Task<bool> CheckUserById(Guid userId);
    Task InsertUserAsync(UserEntity user);
    Task SaveAsync();
}