using PriceNegotiationApp.Models;
using PriceNegotiationApp.Models.Dtos;

namespace PriceNegotiationApp.Services.Interfaces;

public interface IIdentityService
{
    string GenerateToken(UserResponse user);
    string GenerateToken(TokenGeneratorRequest request);
}