using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using EzMap.Domain.Dtos;
using EzMap.Domain.Models;
using EzMap.IntegrationTest.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace EzMap.IntegrationTest;

public class AuthenticationControllerTest
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
            BCrypt.Net.BCrypt.HashPassword("ThHo!0322")));
        await dbContext.SaveChangesAsync();
        // act
        var response = await client.PostAsJsonAsync("api/user/signin",
            new UserSignInDto()
                { Username = "redbull", Password = "ThHo!0322" });

        var content = await response.Content.ReadAsStringAsync();
        // assert 
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotEmpty(content);
    }

    [Fact]
    public async Task SignIn_ReturnsUnauthorizedError_WhenIncorrectCredentialProvided()
    {
        // arrange 
        var app = new TestWebAppFactory<Program>();
        var client = app.CreateClient();
        using var scope = app.Services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<EzMapContext>();
        dbContext.Add(new User("bytetum", "redbull", "bytetum@gmail.com",
            BCrypt.Net.BCrypt.HashPassword("ThHo!0322")));
        await dbContext.SaveChangesAsync();
        // act
        var response = await client.PostAsJsonAsync("api/user/signin",
            new UserSignInDto()
                { Username = "redbull", Password = "ThHoThHo!" });

        // assert 
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
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
            UserName = "bytetum", DisplayName = "hoangml", Email = "notdeciced@gmail.com", Password = "ThHo|0322",
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
    
    [Fact]
    public async Task SignUp_InvalidPasswordViolatedThreeComplixityCheck_BadRequestAndThreeErrorMessagesReturn()
    {
        var app = new TestWebAppFactory<Program>();
        var client = app.CreateClient();
        using var scope = app.Services.CreateScope();
        var userSignUpDto = new UserCreationDto()
        {
            UserName = "bytetum", DisplayName = "hoangml", Email = "notdeciced@gmail.com", Password = "aaaaaa",
        };
        
        var expectedErrorMessages = new List<string>
        {
            "Password must be between 8 and 16 characters long.",
            "Password must contain at least one uppercase letter.",
            "Password must contain at least one digit.",
            "Password must contain at least one special character."
        };
        
        var response = await client.PostAsJsonAsync("api/user/SignUp", userSignUpDto);

        var responseString = await response.Content.ReadAsStringAsync();

        var errorObject = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(responseString, new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        });

        var comparingProperty = nameof(UserSignInDto.Password);
        
        Assert.True(errorObject?[comparingProperty].Count == 4);

        Assert.Equal(expectedErrorMessages, errorObject["Password"]);
    }
}