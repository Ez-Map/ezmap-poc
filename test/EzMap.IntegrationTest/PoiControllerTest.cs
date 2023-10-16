using System.Net;
using System.Net.Http.Json;
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
        // arrange
        var app = new TestWebAppFactory<Program>();
        var client = app.CreateClient();
        using var scope = app.Services.CreateScope();
        var poi = new PoiCreateDto
        (
            "THANH NUMBER FAV PLACE",
            "citygarden"
        );

        // act
        var response = await client.PostAsJsonAsync("poi/CreatePoi", poi);
       
        
        // assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var dbContext = scope.ServiceProvider.GetRequiredService<EzMapContext>();
        var dbPoi = dbContext.Pois.FirstOrDefault(p => p.Name == poi.Name && p.Address == poi.Address);
        
        Assert.NotNull(dbPoi);
        
    }

    [Fact]
    public async Task UpdatePoi_CorrectDataProvided_PoiShouldBeUpdated()
    {
        // arrange
        var app = new TestWebAppFactory<Program>();
        var client = app.CreateClient();
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EzMapContext>();

        var user = new User("thanh", "thanh", "thanh", "thanh");
        dbContext.Users.Add(user);
        var poi = new Poi("home", "59 ntt", user.Id);
        dbContext.Pois.Add(poi);

        await dbContext.SaveChangesAsync();
        
        var updateDto = new PoiUpdateDto
        (
            poi.Id,
            "THANH NUMBER FAV PLACE",
            "citygarden"
        );
        
        // act
        var response = await client.PutAsJsonAsync($"api/poi/{poi.Id}", updateDto);
        
        // assert
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

       
        var user = new User("thanh", "thanh", "thanh", "thanh");
        dbContext.Users.Add(user);
        var poi = new Poi("home", "59 ntt", user.Id);
        dbContext.Pois.Add(poi);

        await dbContext.SaveChangesAsync();
        
              
        // act
        using var response = await client.DeleteAsync($"api/poi/{poi.Id}");
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        // assert
      
    }
}