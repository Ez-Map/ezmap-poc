using System.Net;
using System.Text.Json;
using Azure;
using EzMap.Domain.Dtos;
using EzMap.Domain.Models;
using EzMap.Domain.Repositories;
using EzMap.IntegrationTest.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace EzMap.IntegrationTest;

public class TagControllerTest
{
    [Fact]
    public async Task CreateTag_CorrectDataProvided_TagShouldBeCreated()
    {
        var app = new TestWebAppFactory<Program>();
        var client = app.CreateClient();
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EzMapContext>();

        var token = await TestHelper.GetDefaultUserToken(client);

        var tag = new TagCreateDto
        (
            "THANH NUMBER FAV PLACE",
            "citygarden"
        );

        var response = await client.RequestAsJsonAsyncWithToken(HttpMethod.Post, "api/tag", token, tag);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var dbTag = dbContext.Tags.FirstOrDefault(t => t.Name == tag.Name && t.Description == tag.Description);

        Assert.NotNull(dbTag);
    }

    [Fact]
    public async Task CreateTag_GreaterThan50CharNameProvided_ErrorMessageShow()
    {
        var app = new TestWebAppFactory<Program>();
        var client = app.CreateClient();
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EzMapContext>();

        var token = await TestHelper.GetDefaultUserToken(client);

        var user = new User("thanh", "thanh", "thanh", "thanh");
        dbContext.Users.Add(user);

        var tag = new Tag("home", "59 ntt", TestUser.DefaultUser.Id);
        dbContext.Tags.Add(tag);

        await dbContext.SaveChangesAsync();

        var updateDto = new TagUpdateDto
        (
            tag.Id,
            "citygarden",
            "9VBJRFiYcF9gFeTZGSksaTMavgWTPG4Ep2pFYHqzy5i5hNDpkaa"
        );

        var response = await client.RequestAsJsonAsyncWithToken(HttpMethod.Put, $"api/tag/{tag.Id}", token, updateDto);

        var responseString = await response.Content.ReadAsStringAsync();

        var errorMessage = JsonSerializer.Deserialize<ProblemDetails>(responseString, new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        });

        var descriptionError = errorMessage.Errors[nameof(TagCreateDto.Name)][0];

        Assert.Contains($"'{nameof(TagCreateDto.Name)}'".ToLower(), descriptionError.ToLower());
    }

    [Fact]
    public async Task UpdateTag_CorrectDataProvided_TagShouldBeUpdated()
    {
        var app = new TestWebAppFactory<Program>();
        var client = app.CreateClient();
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EzMapContext>();

        var token = await TestHelper.GetDefaultUserToken(client);

        var user = new User("thanh", "thanh", "thanh", "thanh");
        dbContext.Users.Add(user);

        var tag = new Tag("home", "59 ntt", TestUser.DefaultUser.Id);
        dbContext.Tags.Add(tag);

        await dbContext.SaveChangesAsync();

        var updateDto = new TagUpdateDto
        (
            tag.Id,
            "THANH NUMBER FAV PLACE",
            "citygarden"
        );

        var response = await client.RequestAsJsonAsyncWithToken(HttpMethod.Put, $"api/tag/{tag.Id}", token, updateDto);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        response.EnsureSuccessStatusCode();
        var dbTag = dbContext.Tags.FirstOrDefault(t =>
            t.Name == updateDto.Name && t.Description == updateDto.Description);

        Assert.NotNull(dbTag);
    }

    [Fact]
    public async Task UpdateTag_EmptyTagIdProvided_ErrorMessageShow()
    {
        var app = new TestWebAppFactory<Program>();
        var client = app.CreateClient();
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EzMapContext>();

        var token = await TestHelper.GetDefaultUserToken(client);

        var user = new User("thanh", "thanh", "thanh", "thanh");
        dbContext.Users.Add(user);

        var tag = new Tag("home", "59 ntt", TestUser.DefaultUser.Id);
        dbContext.Tags.Add(tag);

        await dbContext.SaveChangesAsync();

        var updateDto = new TagUpdateDto
        (
            Guid.Empty, 
            "THANH NUMBER FAV PLACE",
            "citygarden"
        );

        var response = await client.RequestAsJsonAsyncWithToken(HttpMethod.Put, $"api/tag/{tag.Id}", token, updateDto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        
        var responseString = await response.Content.ReadAsStringAsync();

        var errors = JsonSerializer.Deserialize<ProblemDetails>(responseString, new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        });

        var comparingProperty = nameof(TagUpdateDto.Id);

        var idErrorMessage = errors.Errors[comparingProperty][0];

        Assert.Contains($"{comparingProperty}".ToLower(), idErrorMessage.ToLower());
    }

    [Fact]
    public async Task DeleteTag_CorrectDataProvided_PoiShouldBeDeleted()
    {
        var app = new TestWebAppFactory<Program>();
        var client = app.CreateClient();
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EzMapContext>();
        var token = await TestHelper.GetDefaultUserToken(client);
        var user = new User("thanh", "thanh", "thanh", "thanh");
        dbContext.Users.Add(user);
        var tag = new Tag("home", "59 ntt", user.Id);
        dbContext.Tags.Add(tag);
        await dbContext.SaveChangesAsync();
        using var response =
            await client.RequestAsJsonAsyncWithToken<object>(HttpMethod.Delete, $"api/tag/{tag.Id}", token);
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetTag_GuidProvided_DetailOfTagShouldBeFound()
    {
        var app = new TestWebAppFactory<Program>();
        var client = app.CreateClient();
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EzMapContext>();
        var token = await TestHelper.GetDefaultUserToken(client);

        var tag = new Tag("cafe", "plact where we can find a drink and chill", TestUser.DefaultUser.Id);
        dbContext.Tags.Add(tag);

        await dbContext.SaveChangesAsync();

        using var response =
            await client.RequestAsJsonAsyncWithToken<object>(HttpMethod.Get, $"api/tag/{tag.Id}", token);

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseTag = JsonSerializer.Deserialize<Tag>(await response.Content.ReadAsStringAsync(),
            new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

        Assert.Equal(tag.Name, responseTag.Name);
        Assert.Equal(tag.Description, responseTag.Description);
    }

    [Fact]
    public async Task GetListOfTag_GuidProvided_DetailOfTagShouldBeFound()
    {
        var app = new TestWebAppFactory<Program>();
        var client = app.CreateClient();
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EzMapContext>();
        var token = await TestHelper.GetDefaultUserToken(client);


        var tag = new Tag("home", "59 ntt", TestUser.DefaultUser.Id);
        dbContext.Tags.Add(tag);

        await dbContext.SaveChangesAsync();

        using var response = await client.RequestAsJsonAsyncWithToken<object>(HttpMethod.Get, $"api/tag/", token);
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);


        var responsePoi = JsonSerializer.Deserialize<List<Tag>>(await response.Content.ReadAsStringAsync(),
            new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });


        Assert.Equal(tag.Name, responsePoi[0].Name);
        Assert.Equal(tag.Description, responsePoi[0].Description);
    }

    [Fact]
    public async Task SearchTag_KeywordProvided_DetailOfTagShouldBeFound()
    {
        var app = new TestWebAppFactory<Program>();
        var client = app.CreateClient();
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EzMapContext>();
        var token = await TestHelper.GetDefaultUserToken(client);

        // create a poi to be searched later
        var tag1 = new Tag("home", "59 ntt", TestUser.DefaultUser.Id);
        var tag2 = new Tag("office", "Hue", TestUser.DefaultUser.Id);
        var tag3 = new Tag("bar", "ruman", TestUser.DefaultUser.Id);
        dbContext.Tags.AddRange(tag1, tag2, tag3);

        await dbContext.SaveChangesAsync();

        using var response =
            await client.RequestAsJsonAsyncWithToken<object>(HttpMethod.Get, $"api/tag/search?keyword={tag1.Name}",
                token);

        response.EnsureSuccessStatusCode();

        var responseTag = JsonSerializer.Deserialize<List<Tag>>(await response.Content.ReadAsStringAsync(),
            new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

        Assert.True(responseTag.Count == 1);
        Assert.Equal(tag1.Name, responseTag[0].Name);
        Assert.Equal(tag1.Description, responseTag[0].Description);
    }
}