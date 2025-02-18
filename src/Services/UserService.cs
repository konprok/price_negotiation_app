using FluentValidation;
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
    private readonly IValidator<UserRegisterDto> _userRegisterValidator;
    public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher, IValidator<UserRegisterDto> userRegisterValidator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _userRegisterValidator = userRegisterValidator;
    }

    public async Task<UserResponse> PostUser(UserRegisterDto user)
    {
        var validationResult = await _userRegisterValidator.ValidateAsync(user);
        if (!validationResult.IsValid)
        {
            var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new InvalidArgumentException(errors);
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
        if (userEmail == null) throw new InvalidArgumentException(ErrorMessages.InvalidUser);
        var user = await _userRepository.GetUser(userEmail);
        if (user == null)
        {
            throw new NotFoundException(ErrorMessages.UserNotFound);
        }
        if (!_passwordHasher.Verify(password, user.PasswordHash))
        {
            throw new InvalidArgumentException(ErrorMessages.InvalidPassword);
        }
        
        return new UserResponse(user);
    }

    public async Task<UserResponse> GetUser(Guid userId)
    {
        
        var userEntity = await _userRepository.GetUser(userId);
        if (userEntity == null)
        {
            throw new NotFoundException(ErrorMessages.UserNotFound);
        }

        var user = new UserResponse(userEntity);

        return user;
    }
    
}