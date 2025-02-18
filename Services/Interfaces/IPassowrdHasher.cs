namespace PriceNegotiationApp.Services.Interfaces;

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string requestPassword, string userPasswordHash);
}