using System.Net;
using System.Text.Json;
using EzMap.Domain.Constants;
using EzMap.Domain.Dtos;
using EzMap.Domain.Models;
using EzMap.IntegrationTest.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace EzMap.IntegrationTest;

public class PoiCollectionControllerTest
{
    [Fact]
    public async Task CreatePoiCollection_GreaterThan100CharNameProvided_ErrorMessageShow()
    {
        var app = new TestWebAppFactory<Program>();
        var client = app.CreateClient();
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EzMapContext>();


        var token = await TestHelper.GetDefaultUserToken(client);


        var poiCol = new PoiCollectionCreateDto
        (
            "9VBJRFiYcF9gFeTZGSksaTMavgWTPG4Ep2pFYHqzy5i5hNDpkvsaa9VBJRFiYcF9gFeTZGSksaTMavgWTPG4Ep2pFYHqzy5i5hNDpkvsaa",
            "citygarden"
        );

        var response = await client.RequestAsJsonAsyncWithToken(HttpMethod.Post, "api/poicollection/", token, poiCol);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var responseString = await response.Content.ReadAsStreamAsync();

        var comparingProperty = nameof(PoiCollectionCreateDto.Name).ToLower();

        var nameShowingMessage = JsonSerializer.Deserialize<ProblemDetails>(responseString, new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        });
        Assert.Contains(comparingProperty, nameShowingMessage.Errors[nameof(PoiCollectionCreateDto.Name)][0].ToLower());
    }

    [Fact]
    public async Task CreatePoiCollection_CorrectDataProvided_PoiShouldBeCreated()
    {
        var app = new TestWebAppFactory<Program>();
        var client = app.CreateClient();
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EzMapContext>();


        var token = await TestHelper.GetDefaultUserToken(client);


        var poiCol = new PoiCollectionCreateDto
        (
            "THANH NUMBER FAV PLACE",
            "citygarden"
        );


        var response = await client.RequestAsJsonAsyncWithToken(HttpMethod.Post, "api/poicollection/", token, poiCol);


        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var dbPoiCol =
            dbContext.PoiCollections.FirstOrDefault(p => p.Name == poiCol.Name && p.Description == poiCol.Description);

        Assert.NotNull(dbPoiCol);
    }

    [Fact]
    public async Task UpdatePoiCollection_CorrectDataProvided_PoiColShouldBeUpdated()
    {
        var app = new TestWebAppFactory<Program>();
        var client = app.CreateClient();
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EzMapContext>();

        var token = await TestHelper.GetDefaultUserToken(client);

        var user = new User("thanh", "thanh", "thanh", "thanh");
        dbContext.Users.Add(user);

        var poiCol = new PoiCollection("home", "59 ntt", TestUser.DefaultUser.Id);
        dbContext.PoiCollections.Add(poiCol);

        await dbContext.SaveChangesAsync();

        var updateDto = new PoiCollectionUpdateDto(
            poiCol.Id,
            "THANH NUMBER FAV PLACE",
            "citygarden",
            PoiEnum.ViewType.Map
        );

        var response =
            await client.RequestAsJsonAsyncWithToken(HttpMethod.Put, $"api/poicollection/{poiCol.Id}", token,
                updateDto);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var dbPoiCol =
            dbContext.PoiCollections.FirstOrDefault(t =>
                t.Name == updateDto.Name && t.Description == updateDto.Description);

        Assert.NotNull(dbPoiCol);
    }

    [Fact]
    public async Task UpdatePoiCollectio_EmptyPoiColIdProvided_EmptyIdErrorMessageShouldBeShowed()
    {
        var app = new TestWebAppFactory<Program>();
        var client = app.CreateClient();
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EzMapContext>();

        var token = await TestHelper.GetDefaultUserToken(client);

        var user = new User("thanh", "thanh", "thanh", "thanh");
        dbContext.Users.Add(user);

        var poiCol = new PoiCollection("home", "59 ntt", TestUser.DefaultUser.Id);
        dbContext.PoiCollections.Add(poiCol);

        await dbContext.SaveChangesAsync();

        var updateDto = new PoiCollectionUpdateDto(
            Guid.Empty,
            "THANH NUMBER FAV PLACE",
            "citygarden",
            PoiEnum.ViewType.Map
        );

        var response =
            await client.RequestAsJsonAsyncWithToken(HttpMethod.Put, $"api/poicollection/{poiCol.Id}", token,
                updateDto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var comparingProperty = nameof(PoiCollectionUpdateDto.Id);

        var responseString = await response.Content.ReadAsStringAsync();

        var errorMessages = JsonSerializer.Deserialize<ProblemDetails>(responseString, new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true,
        });

        var idErrorMessage = errorMessages?.Errors[comparingProperty][0];

        Assert.Contains($"{comparingProperty}".ToLower(), idErrorMessage?.ToLower());
    }

    [Fact]
    public async Task DeletePoiCollection_GuidProvided_PoiColShouldBeDeleted()
    {
        var app = new TestWebAppFactory<Program>();
        var client = app.CreateClient();
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EzMapContext>();

        var token = await TestHelper.GetDefaultUserToken(client);

        var user = new User("thanh", "thanh", "thanh", "thanh");
        dbContext.Users.Add(user);

        var poiCol = new PoiCollection("home", "59 ntt", TestUser.DefaultUser.Id);
        dbContext.PoiCollections.Add(poiCol);

        await dbContext.SaveChangesAsync();

        using var response =
            await client.RequestAsJsonAsyncWithToken<object>(HttpMethod.Delete, $"api/poicollection/{poiCol.Id}",
                token);

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetPoiCol_GuidProvided_DetailOfPoiCOlShouldBeFound()
    {
        var app = new TestWebAppFactory<Program>();
        var client = app.CreateClient();
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EzMapContext>();

        var token = await TestHelper.GetDefaultUserToken(client);

        var user = new User("thanh", "thanh", "thanh", "thanh");
        dbContext.Users.Add(user);

        var poiCol = new PoiCollection("home", "59 ntt", TestUser.DefaultUser.Id);
        dbContext.PoiCollections.Add(poiCol);

        await dbContext.SaveChangesAsync();

        var response =
            await client.RequestAsJsonAsyncWithToken<object>(HttpMethod.Get, $"api/poicollection/{poiCol.Id}", token);
        response.EnsureSuccessStatusCode();
        var responsePoiCol = JsonSerializer.Deserialize<PoiCollection>(await response.Content.ReadAsStringAsync(),
            new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

        Assert.Equal(poiCol.Name, responsePoiCol.Name);
        Assert.Equal(poiCol.Description, responsePoiCol.Description);
    }

    [Fact]
    public async Task SearchPoiCol_KeywordProvided_DetailOfPoiColShouldBeFound()
    {
        var app = new TestWebAppFactory<Program>();
        var client = app.CreateClient();
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EzMapContext>();
        var token = await TestHelper.GetDefaultUserToken(client);

        // create a poi to be searched later
        var poiCol1 = new PoiCollection("home", "59 ntt", TestUser.DefaultUser.Id);
        var poiCol2 = new PoiCollection("office", "Hue", TestUser.DefaultUser.Id);
        var poiCol3 = new PoiCollection("bar", "ruman", TestUser.DefaultUser.Id);
        dbContext.PoiCollections.AddRange(poiCol1, poiCol2, poiCol3);

        await dbContext.SaveChangesAsync();

        using var response =
            await client.RequestAsJsonAsyncWithToken<object>(HttpMethod.Get,
                $"api/poicollection/search?keyword={poiCol2.Name}",
                token);

        response.EnsureSuccessStatusCode();

        var responsePoiCol = JsonSerializer.Deserialize<List<PoiCollection>>(await response.Content.ReadAsStringAsync(),
            new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

        Assert.True(responsePoiCol.Count == 1);
        Assert.Equal(poiCol2.Name, responsePoiCol[0].Name);
        Assert.Equal(poiCol2.Description, responsePoiCol[0].Description);
    }
}