using BCrypt.Net;
using EzMap.Domain.Dtos;
using EzMap.Domain.Models;
using Microsoft.EntityFrameworkCore;
using BC = BCrypt.Net.BCrypt;


namespace EzMap.Domain.Repositories;
public interface IUserRepository
{
    Task<bool> CreateUser(UserCreationDto userCreationDto);

    Task<Guid?> SignIn(UserSignInDto userSignInDto);
}

public class UserRepository : IUserRepository
{
    private readonly EzMapContext _dbContext;

    public UserRepository(EzMapContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> CreateUser(UserCreationDto userCreationDto)
    {
        string passwordHash =  BC.HashPassword(userCreationDto.Password);
        var user = new User(userCreationDto.DisplayName, userCreationDto.UserName, userCreationDto.Email,
            passwordHash);
    
        _dbContext.Users.Add(user);

        return await _dbContext.SaveChangesAsync() > 0;
    }

    public async Task<Guid?> SignIn(UserSignInDto userSignInDto)
    {
         var dbUser = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserName == userSignInDto.Username);

         if (dbUser != null)
         {
             return BC.Verify(userSignInDto.Password, dbUser.Password) ? dbUser.Id : null;
         }
         return null;
    }
    
    
}