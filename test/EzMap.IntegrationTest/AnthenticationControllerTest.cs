using System.Net;
using System.Net.Http.Json;
using EzMap.Domain;
using EzMap.Domain.Dtos;
using EzMap.Domain.Models;
using EzMap.IntegrationTest.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EzMap.IntegrationTest;

public class AnthenticationControllerTest
{
    [Fact]
    public async Task SignIn_CorrectUsernamePassword_TokenShouldBeGranted()
    {
        // arrange 
        var app = new TestWebAppFactory<Program>();
        var client = app.CreateClient();
        using var scope = app.Services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<EzMapContext>();
        dbContext.Add(new User("bytetum", "redbull", "bytetum@gmail.com",
            BCrypt.Net.BCrypt.HashPassword("strawberry")));
        await dbContext.SaveChangesAsync();

        var users = await dbContext.Users.ToListAsync();
        // act
        var response = await client.PostAsJsonAsync("api/user/signin",
            new UserSignInDto()
                { Username = "redbull", Password = "strawberry" });

        var content = await response.Content.ReadAsStringAsync();
        // assert 
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotEmpty(content);
    }

    [Fact]
    public async Task SignUp_ProvideCorrectData_UserRecordCreated()
    {
        // arrange 
        var app = new TestWebAppFactory<Program>();
        var client = app.CreateClient();
        using var scope = app.Services.CreateScope();
        var user = new UserCreationDto()
        {
            UserName = "bytetum", DisplayName = "hoangml", Email = "notdeciced@gmail.com", Password = "bytetum",
        };

        // act
        // client send http request including a valid user 
        var response = await client.PostAsJsonAsync("api/user/signup", user);


        // assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var dbContext = scope.ServiceProvider.GetRequiredService<EzMapContext>();
        var dbUser = dbContext.Users.FirstOrDefault(x =>
            x.UserName == user.UserName && x.Email == user.Email && x.DisplayName == user.DisplayName);

        // successful message returned
        Assert.True(BCrypt.Net.BCrypt.Verify(user.Password, dbUser.Password));
    }
}