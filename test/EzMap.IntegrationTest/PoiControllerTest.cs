using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using EzMap.Domain;
using EzMap.Domain.Dtos;
using EzMap.Domain.Models;
using EzMap.IntegrationTest.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.Resources;

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
    public async Task UpdatePoi_IncorrectDataProvided_CorrectMessageShouldBeReturned()
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
            "aaaaaaaa",
            "City Garden"
        );


        var response = await client.RequestAsJsonAsyncWithToken(HttpMethod.Put, $"api/poi/{poi.Id}", token, updateDto);

        var responseString = await response.Content.ReadAsStringAsync();

        var errors = JsonSerializer.Deserialize<ProblemDetails>(responseString, new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true,
        });

        if (errors is not null)
        {
            var nameErrorMessage = TestHelper.GetErrorMessageOfAProperty(errors.Errors, nameof(PoiUpdateDto.Name), 0);

            Assert.NotNull(nameErrorMessage);
            Assert.Contains($"'{nameof(PoiUpdateDto.Name)}' must not be empty.".ToLower(), nameErrorMessage?.ToLower());
        }
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