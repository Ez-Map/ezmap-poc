using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using EzMap.Domain.Dtos;
using EzMap.Domain.Models;
using EzMap.IntegrationTest.Infrastructure;
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
            UserName = "bytetum", DisplayName = "hoangml", Email = "notdeciced@gmail.com", Password = "ThHo!0322",
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
    public async Task SignUp_InvalidPasswordProvided_BadRequestAndErrorMessageReturn()
    {
        var app = new TestWebAppFactory<Program>();
        var client = app.CreateClient();
        using var scope = app.Services.CreateScope();
        var userSignUpDto = new UserCreationDto()
        {
            UserName = "bytetum", DisplayName = "hoangml", Email = "notdeciced@gmail.com", Password = "Aaaaaaa!",
        };

        var response = await client.PostAsJsonAsync("api/user/SignUp", userSignUpDto);

        var responseString = await response.Content.ReadAsStringAsync();

        var errorObject = JsonSerializer.Deserialize<ProblemDetails>(responseString, new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        });

        var comparingProperty = nameof(UserSignInDto.Password);

        var returnedErrorMessage = errorObject.Errors[comparingProperty][0];

        var expectedErrorMessage = "Your password must contain at least one number.";

        Assert.True(expectedErrorMessage.Contains(returnedErrorMessage, StringComparison.CurrentCultureIgnoreCase));
    }
}