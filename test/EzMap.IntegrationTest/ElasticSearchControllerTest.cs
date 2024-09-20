using System.Net;
using System.Text;
using System.Text.Json;
using EzMap.Domain;
using EzMap.Domain.Dtos;
using EzMap.Domain.Models;
using EzMap.IntegrationTest.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Nest;

namespace EzMap.IntegrationTest;

public class ElasticSearchControllerTest
{
    [Fact]
    public async Task CreateIndex_ReturnsOkResult()
    {
        var app = new TestWebAppFactory<Program>();     
        var client = app.CreateClient();
       
        var indexName = "thanh";
        using var scope = app.Services.CreateScope();
        var elasticClient = scope.ServiceProvider.GetRequiredService<IElasticClient>();
        var token = await TestHelper.GetDefaultUserToken(client);
        // Delete the index if it already exists
        await elasticClient.Indices.DeleteAsync(indexName);

        var result = await client.RequestAsJsonAsyncWithTokenAndUrlParam<object>(HttpMethod.Post, "api/PoiElasticSearch/index",
           $"{indexName}", token, null);

        // Verify that the index was created
        var indexExistsResponse = await elasticClient.Indices.ExistsAsync(indexName);
        Assert.True(indexExistsResponse.Exists);
    }

    [Fact]
    public async Task AddOrUpdateBulk_ReturnsOkResult_WhenSuccessful()
    {
        var app = new TestWebAppFactory<Program>();
        var client = app.CreateClient();
        using var scope = app.Services.CreateScope();
        var indexName = "test-index";
        var token = await TestHelper.GetDefaultUserToken(client);
        //var elasticClient = scope.ServiceProvider.GetRequiredService<IElasticClient>();

        // var poi = new PoiCreateDto(
        //     "9VBJRFiYcF9gFeTZGSksaTMavgWTPG4Ep2pFYHqzy5i5hNDpkvaaa",
        //     "citygarden"
        // );
        // var listOfPoiDoc = new List<PoiCreateDto> { poi };
        // var documents = new List<PoiCreateDto>
        // {
        //     new PoiCreateDto("id1", "citygarden1"),
        //     new PoiCreateDto("id2", "citygarden2")
        // };
        // var content = new StringContent(JsonSerializer.Serialize(documents), Encoding.UTF8, "application/json");
        // var response = await client.PostAsync("/api/PoiElasticSearch/", content);
        //Act
        var elasticClient = scope.ServiceProvider.GetRequiredService<IElasticClient>();
        var indexExistsResponse = await elasticClient.Indices.ExistsAsync(indexName);
        Assert.True(indexExistsResponse.Exists);
            var response = await client.RequestAsJsonAsyncWithToken(
                HttpMethod.Post,
                "api/PoiElasticSearch/",
                token,
                new Poi("id2", "citygarden2", Guid.NewGuid())
            );
            
        
        
            Assert.True(response.IsSuccessStatusCode);
    }
    
    [Fact]
    public async Task AddOrUpdate_ReturnsOkResult_WhenSuccessful2()
    {
       
        // Arrange
        var app = new TestWebAppFactory<Program>();
        var client = app.CreateClient();
        using var scope = app.Services.CreateScope();
        var token = await TestHelper.GetDefaultUserToken(client);

        // Act
        
        var indexName = "ThanhHoang";
        var elasticClient = scope.ServiceProvider.GetRequiredService<IElasticClient>();
        await elasticClient.Indices.DeleteAsync(indexName);
        var result = await client.RequestAsJsonAsyncWithTokenAndUrlParam<object>(HttpMethod.Post, "api/PoiElasticSearch/index",
            $"{indexName}", token, null);
        var indexExistsResponse = await elasticClient.Indices.ExistsAsync(indexName);
        Assert.True(indexExistsResponse.Exists);
        
        var poi = new Poi
        {
            Id = Guid.NewGuid(),
            Name = "citygarden2",
            UserId = Guid.NewGuid(),
            User = new User("thanh", "thanh", "a", "b"),
            Address = "a"
        };
        Console.WriteLine($"Sending POI: {JsonSerializer.Serialize(poi)}"); // Log the data being sent

        var response = await client.RequestAsJsonAsyncWithToken(
            HttpMethod.Post,
            "api/PoiElasticSearch/",
            token,
            poi
        );

        // Assert
        var content = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Response Status: {response.StatusCode}");
        Console.WriteLine($"Response Content: {content}");

        if (!response.IsSuccessStatusCode)
        {
            Assert.True(false, $"Expected success status code, but got {response.StatusCode}. Response content: {content}");
        }

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    
    [Fact]
    public async Task AddOrUpdate_ReturnsOkResult_WhenSuccessful3()
    {
       
        // Arrange
        var app = new TestWebAppFactory<Program>();
        var client = app.CreateClient();
        using var scope = app.Services.CreateScope();
        var token = await TestHelper.GetDefaultUserToken(client);

        // Act
        
        var indexName = "thanh2";
        var elasticClient = scope.ServiceProvider.GetRequiredService<IElasticClient>();
        var result = await client.RequestAsJsonAsyncWithTokenAndUrlParam<object>(HttpMethod.Post, "api/PoiElasticSearch/index",
            $"{indexName}", token, null);
        var indexExistsResponse = await elasticClient.Indices.ExistsAsync(indexName);
        Assert.True(indexExistsResponse.Exists);
        
        var poi = new Poi
        {
            Id = Guid.NewGuid(),
            Name = "c",
            UserId = Guid.NewGuid(),
            User = new User("c", "c", "c", "c"),
            Address = "a"
        };
        Console.WriteLine($"Sending POI: {JsonSerializer.Serialize(poi)}"); // Log the data being sent

        var response = await client.RequestAsJsonAsyncWithToken(
            HttpMethod.Post,
            "api/PoiElasticSearch/",
            token,
            poi
        );

        // Assert
        var content = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Response Status: {response.StatusCode}");
        Console.WriteLine($"Response Content: {content}");

        if (!response.IsSuccessStatusCode)
        {
            Assert.True(false, $"Expected success status code, but got {response.StatusCode}. Response content: {content}");
        }

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}