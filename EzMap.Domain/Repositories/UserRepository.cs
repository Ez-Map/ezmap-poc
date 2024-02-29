using BCrypt.Net;
using EzMap.Domain.Dtos;
using EzMap.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using BC = BCrypt.Net.BCrypt;


namespace EzMap.Domain.Repositories;

public interface IUserRepository
{
    Task<bool> CreateUser(UserCreationDto userCreationDto, CancellationToken token = default);

    Task<Guid?> SignIn(UserSignInDto userSignInDto, CancellationToken token = default);
}

public class UserRepository : IUserRepository
{
    private readonly EzMapContext _dbContext;

    private readonly ILogger<UserRepository> _logger;

    public UserRepository(EzMapContext dbContext, ILogger<UserRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<bool> CreateUser(UserCreationDto userCreationDto, CancellationToken token = default)
    {
        string passwordHash = BC.HashPassword(userCreationDto.Password);
        var user = new User(userCreationDto.DisplayName, userCreationDto.UserName, userCreationDto.Email,
            passwordHash);

        _dbContext.Users.Add(user);

        await _dbContext.SaveChangesAsync(token);
        
        _logger.LogInformation("User created successfully: {Username}", userCreationDto.UserName);
        
        return true;
    }

    public async Task<Guid?> SignIn(UserSignInDto userSignInDto, CancellationToken token = default)
    {
        var dbUser =
            await _dbContext.Users.FirstOrDefaultAsync(x => x.UserName == userSignInDto.Username,
                cancellationToken: token);

        if (dbUser is not null)
        {
            return BC.Verify(userSignInDto.Password, dbUser.Password) ? dbUser.Id : null;
        }

        return null;
    }
}