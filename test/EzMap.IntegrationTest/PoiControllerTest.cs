using System.Net;
using System.Text.Json;
using EzMap.Domain;
using EzMap.Domain.Dtos;
using EzMap.Domain.Models;
using EzMap.IntegrationTest.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace EzMap.IntegrationTest;

public class PoiControllerTest
{
    [Fact]
    public async Task CreatePoi_CorrectDataProvided_PoiShouldBeCreated()
    {
        var app = new TestWebAppFactory<Program>();
        var client = app.CreateClient();
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EzMapContext>();
        var token = await TestHelper.GetDefaultUserToken(client);

        var poi = new PoiCreateDto
        (
            "THANH NUMBER FAV PLACE",
            "citygarden"
        );

        var response = await client.RequestAsJsonAsyncWithToken(HttpMethod.Post, "api/poi/", token, poi);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var dbPoi = dbContext.Pois.FirstOrDefault(p => p.Name == poi.Name && p.Address == poi.Address);

        Assert.NotNull(dbPoi);
    }

    [Fact]
    public async Task CreatePoi_GreaterThan50CharNameProvided_ErrorMessageShow()
    {
        var app = new TestWebAppFactory<Program>();
        var client = app.CreateClient();
        using var scope = app.Services.CreateScope();
        var token = await TestHelper.GetDefaultUserToken(client);

        var poi = new PoiCreateDto
        (
            "9VBJRFiYcF9gFeTZGSksaTMavgWTPG4Ep2pFYHqzy5i5hNDpkvaaa",
            "citygarden"
        );

        var response = await client.RequestAsJsonAsyncWithToken(HttpMethod.Post, "api/poi/", token, poi);

        var responseString = await response.Content.ReadAsStringAsync();

        var errorMessage = JsonSerializer.Deserialize<ProblemDetails>(responseString, new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        });

        var returnedErrorMessage = errorMessage.Errors[nameof(PoiCreateDto.Name)][0];

        var expectedErrorMessage = $"The length of '{nameof(PoiCreateDto.Name)}' must be 50 characters or fewer";

        Assert.True(returnedErrorMessage.Contains(expectedErrorMessage, StringComparison.CurrentCultureIgnoreCase));
    }

    [Fact]
    public async Task UpdatePoi_IncorrectDtoProvided_CorrespondingMessageShouldBeShowed()
    {
        var app = new TestWebAppFactory<Program>();
        var client = app.CreateClient();
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EzMapContext>();

        var token = await TestHelper.GetDefaultUserToken(client);

        var user = new User("thanh", "thanh", "thanh", "thanh");
        dbContext.Users.Add(user);
        var poi = new Poi("home", "59 ntt", TestUser.DefaultUser.Id);
        dbContext.Pois.Add(poi);
        await dbContext.SaveChangesAsync();

        var updateDto = new PoiUpdateDto
        (
            Guid.Empty,
            "",
            ""
        );

        var response = await client.RequestAsJsonAsyncWithToken(HttpMethod.Put, $"api/poi/{poi.Id}", token, updateDto);

        var responseString = await response.Content.ReadAsStringAsync();

        var errors = JsonSerializer.Deserialize<ProblemDetails>(responseString, new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true,
        });

        var returnedIdErrorMessage = errors?.Errors[nameof(PoiUpdateDto.Id)][0];

        var returnedNameErrorMessage = errors?.Errors[nameof(PoiUpdateDto.Name)][0];

        var returnedAddressErrorMessage = errors?.Errors[nameof(PoiUpdateDto.Address)][0];

        var expectedNameErrorMessage = $"'{nameof(PoiUpdateDto.Name)}' must not be empty.";

        var expectedIdErrorMessage = $"'{nameof(PoiUpdateDto.Id)}' must not be empty";

        var expectedAddressMessage = $"'{nameof(PoiUpdateDto.Address)}' must not be empty";

        Assert.True(returnedNameErrorMessage?.Contains(expectedNameErrorMessage,
            StringComparison.CurrentCultureIgnoreCase));

        Assert.True(returnedIdErrorMessage?.Contains(expectedIdErrorMessage,
            StringComparison.CurrentCultureIgnoreCase));

        Assert.True(returnedAddressErrorMessage?.Contains(expectedAddressMessage,
            StringComparison.CurrentCultureIgnoreCase));
    }

    [Fact]
    public async Task UpdatePoi_CorrectDataProvided_PoiShouldBeUpdated()
    {
        var app = new TestWebAppFactory<Program>();
        var client = app.CreateClient();
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EzMapContext>();

        var token = await TestHelper.GetDefaultUserToken(client);

        var user = new User("thanh", "thanh", "thanh", "thanh");
        dbContext.Users.Add(user);
        var poi = new Poi("home", "59 ntt", TestUser.DefaultUser.Id);
        dbContext.Pois.Add(poi);
        await dbContext.SaveChangesAsync();

        var updateDto = new PoiUpdateDto
        (
            poi.Id,
            "THANH NUMBER FAV PLACE",
            "citygarden"
        );


        var response = await client.RequestAsJsonAsyncWithToken(HttpMethod.Put, $"api/poi/{poi.Id}", token, updateDto);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var dbPoi = dbContext.Pois.FirstOrDefault(p => p.Name == updateDto.Name && p.Address == updateDto.Address);

        Assert.NotNull(dbPoi);
    }

    [Fact]
    public async Task DeletePoi_CorrectDataProvided_PoiShouldBeDeleted()
    {
        var app = new TestWebAppFactory<Program>();
        var client = app.CreateClient();
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EzMapContext>();
        var token = await TestHelper.GetDefaultUserToken(client);


        var user = new User("thanh", "thanh", "thanh", "thanh");
        dbContext.Users.Add(user);
        var poi = new Poi("home", "59 ntt", user.Id);
        dbContext.Pois.Add(poi);

        await dbContext.SaveChangesAsync();
        using var response =
            await client.RequestAsJsonAsyncWithToken<object>(HttpMethod.Delete, $"api/poi/{poi.Id}", token);
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetPoi_GuidProvided_DetailOfPoiShouldBeFound()
    {
        var app = new TestWebAppFactory<Program>();
        var client = app.CreateClient();
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EzMapContext>();
        var token = await TestHelper.GetDefaultUserToken(client);


        var poi = new Poi("home", "59 ntt", TestUser.DefaultUser.Id);
        dbContext.Pois.Add(poi);

        await dbContext.SaveChangesAsync();

        using var response =
            await client.RequestAsJsonAsyncWithToken<object>(HttpMethod.Get, $"api/poi/{poi.Id}", token);
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responsePoi = JsonSerializer.Deserialize<Poi>(await response.Content.ReadAsStringAsync(),
            new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

        Assert.Equal(poi.Name, responsePoi.Name);
        Assert.Equal(poi.Address, responsePoi.Address);
    }

    [Fact]
    public async Task GetListOfPoi_GuidProvided_DetailOfPoiShouldBeFound()
    {
        var app = new TestWebAppFactory<Program>();
        var client = app.CreateClient();
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EzMapContext>();
        var token = await TestHelper.GetDefaultUserToken(client);


        var poi = new Poi("home", "59 ntt", TestUser.DefaultUser.Id);
        dbContext.Pois.Add(poi);

        await dbContext.SaveChangesAsync();

        using var response = await client.RequestAsJsonAsyncWithToken<object>(HttpMethod.Get, $"api/poi/", token);
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);


        var responsePoi = JsonSerializer.Deserialize<List<Poi>>(await response.Content.ReadAsStringAsync(),
            new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });


        Assert.Equal(poi.Name, responsePoi[0].Name);
        Assert.Equal(poi.Address, responsePoi[0].Address);
    }

    [Fact]
    public async Task SearchPoi_KeywordProvided_DetailofPoiShouldBeFound()
    {
        var app = new TestWebAppFactory<Program>();
        var client = app.CreateClient();
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EzMapContext>();
        var token = await TestHelper.GetDefaultUserToken(client);

        // create a poi to be searched later
        var poi1 = new Poi("home", "59 ntt", TestUser.DefaultUser.Id);
        var poi2 = new Poi("office", "Hue", TestUser.DefaultUser.Id);
        var poi3 = new Poi("bar", "ruman", TestUser.DefaultUser.Id);
        dbContext.Pois.AddRange(poi1, poi2, poi3);

        await dbContext.SaveChangesAsync();

        using var response =
            await client.RequestAsJsonAsyncWithToken<object>(HttpMethod.Get, $"api/poi/search?keyword={poi1.Address}",
                token);

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);


        var responsePoi = JsonSerializer.Deserialize<List<Poi>>(await response.Content.ReadAsStringAsync(),
            new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

        Assert.True(responsePoi.Count == 1);
        Assert.Equal(poi1.Name, responsePoi[0].Name);
        Assert.Equal(poi1.Address, responsePoi[0].Address);
    }
}