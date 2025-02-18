using System.Text.RegularExpressions;
using PriceNegotiationApp.Services.Interfaces;
using PriceNegotiationApp.Models.Dtos;
using PriceNegotiationApp.Database.Entities;
using PriceNegotiationApp.Database.Repositories.Interfaces;
using PriceNegotiationApp.Models.Exceptions;


namespace PriceNegotiationApp.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<UserResponse> PostUser(UserRegisterDto user)
    {
        if (string.IsNullOrEmpty(user.UserName) ||
            string.IsNullOrEmpty(user.Password) ||
            string.IsNullOrEmpty(user.Email))
        {
            throw new InvalidInputException("All fields are required.");
        }

        var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        if (!emailRegex.IsMatch(user.Email))
        {
            throw new InvalidInputException("Invalid email format.");
        }
        
        UserEntity newUser = new()
        {
            UserName = user.UserName,
            Email = user.Email,
            PasswordHash = _passwordHasher.Hash(user.Password)
        };
        await _userRepository.InsertUserAsync(newUser);
        await _userRepository.SaveAsync();

        UserResponse userResponse = new UserResponse(newUser);
        return userResponse;
    }
    public async Task<UserResponse> GetUser(string userEmail, string password)
    {
        var user = await _userRepository.GetUser(userEmail);
        if (!_passwordHasher.Verify(password, user.PasswordHash))
        {
            throw new InvalidPasswordException();
        }

        UserResponse userResponse = new UserResponse(user);
        return userResponse;
    }

    public async Task<UserResponse> GetUser(Guid userId)
    {
        return await _userRepository.GetUser(userId);
    }
}