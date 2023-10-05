using BCrypt.Net;
using EzMap.Domain.Dtos;
using EzMap.Domain.Models;
using Microsoft.EntityFrameworkCore;
using BC = BCrypt.Net.BCrypt;


namespace EzMap.Domain.Repositories;
public interface IUserRepository
{
    Task<bool> CreateUser(UserCreationDto userCreationDto);

    bool SignIn(UserSignInDto userSignInDto);
}

public class UserRepository : IUserRepository
{
    private EzMapContext _dbContext;

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

    public bool SignIn(UserSignInDto userSignInDto)
    {
         var password = _dbContext.Users.Where(x => x.UserName == userSignInDto.Username).Select(x => x.Password).FirstOrDefault();

         var all = _dbContext.Users.ToList();
         
         return BC.Verify(userSignInDto.Password, password);
    }
    
    
}