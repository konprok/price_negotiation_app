using PriceNegotiationApp.Database.Repositories.Interfaces;
using PriceNegotiationApp.Models.Dtos;
using PriceNegotiationApp.Database.Entities;
using Microsoft.EntityFrameworkCore;
using PriceNegotiationApp.Models.Exceptions;
using PriceNegotiationApp.Database.DbContext;

namespace PriceNegotiationApp.Database.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserDbContext _dbContext;

    public UserRepository(UserDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<List<UserEntity>> GetUsersList()
    {
        return await _dbContext.Users.ToListAsync();
    }
    public async Task<UserEntity> GetUserEntity(Guid userId)
    {
        UserEntity? userEntity = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
        if (userEntity == null)
        {
            throw new UserNotFoundException();
        }

        return userEntity;
    }

    public async Task<UserResponse> GetUser(Guid userId)
    {
        UserEntity? userEntity = await _dbContext.Users
            .Where(x => x.Id == userId)
            .SingleOrDefaultAsync();
        if (userEntity == null)
        {
            throw new UserNotFoundException();
        }

        var user = new UserResponse(userEntity);
        return user;
    }
    public async Task<UserEntity> GetUser(string userEmail)
    {
        if (userEmail == null) throw new InvalidUserException();
        UserEntity? user = await _dbContext.Users
            .Where(x => x.Email.ToLower() == userEmail.ToLower())
            .SingleOrDefaultAsync();

        if (user == null)
        {
            throw new UserNotFoundException();
        }

        return user;
    }
    public async Task InsertUserAsync(UserEntity user)
    {
        if (await _dbContext.Users.AnyAsync(x => x.UserName == user.UserName || x.Email == user.Email))
        {
            throw new UserAlreadyExistException();
        }
        await _dbContext.Users.AddAsync(user);
    }
    public async Task SaveAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
    public async Task DeleteUser(Guid userId)
    {
        var user = await _dbContext.Users
            .Where(x => x.Id == userId)
            .SingleOrDefaultAsync();

        if (user == null)
        {
            throw new UserNotFoundException();
        }

        _dbContext.Users.Remove(user);
    }
}