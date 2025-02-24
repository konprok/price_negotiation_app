using FluentValidation;
using PriceNegotiationApp.Services.Interfaces;
using PriceNegotiationApp.Models.Dtos;
using PriceNegotiationApp.Database.Entities;
using PriceNegotiationApp.Database.Repositories.Interfaces;
using PriceNegotiationApp.Models.Exceptions;


namespace PriceNegotiationApp.Services;

public sealed class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IValidator<UserRegisterDto> _userRegisterValidator;

    public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher,
        IValidator<UserRegisterDto> userRegisterValidator)
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

        UserEntity newUser = new(user);
        await _userRepository.InsertUserAsync(newUser);
        await _userRepository.SaveAsync();

        return new UserResponse(newUser);
    }

    public async Task<UserResponse> GetUser(UserLoginDto userLoginDto)
    {
        if (userLoginDto.Email == null) throw new InvalidArgumentException(ErrorMessages.InvalidInput);
        var user = await _userRepository.GetUser(userLoginDto.Email);
        if (user == null)
        {
            throw new NotFoundException(ErrorMessages.UserNotFound);
        }

        if (!_passwordHasher.Verify(userLoginDto.Password, user.PasswordHash))
        {
            throw new InvalidArgumentException(ErrorMessages.InvalidPassword);
        }

        UserResponse userResponse = new UserResponse(user);
        return userResponse;
    }

    public async Task<UserResponse> GetUser(Guid userId)
    {
        var userEntity = await _userRepository.GetUser(userId);
        if (userEntity == null)
        {
            throw new NotFoundException(ErrorMessages.UserNotFound);
        }

        return new UserResponse(userEntity);
    }
}