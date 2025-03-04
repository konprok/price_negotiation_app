using PriceNegotiationApp.Database.Repositories.Interfaces;
using PriceNegotiationApp.Database.Entities;
using Microsoft.EntityFrameworkCore;
using PriceNegotiationApp.Models.Exceptions;
using PriceNegotiationApp.Database.DbContext;

namespace PriceNegotiationApp.Database.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly AppDbContext _dbContext;

    public UserRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<UserEntity>> GetUsersList()
    {
        return await _dbContext.Users.ToListAsync();
    }

    public async Task<UserEntity?> GetUser(Guid userId)
    {
        UserEntity? userEntity = await _dbContext.Users
            .Where(x => x.Id == userId)
            .SingleOrDefaultAsync();

        return userEntity;
    }

    public async Task<UserEntity?> GetUser(string userEmail)
    {
        UserEntity? user = await _dbContext.Users
            .Where(x => x.Email.ToLower() == userEmail.ToLower())
            .SingleOrDefaultAsync();

        return user;
    }

    public async Task InsertUserAsync(UserEntity user)
    {
        if (await _dbContext.Users.AnyAsync(x => x.UserName == user.UserName || x.Email == user.Email))
        {
            throw new InvalidArgumentException(ErrorMessages.UserAlreadyExist);
        }

        await _dbContext.Users.AddAsync(user);
    }

    public async Task SaveAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}