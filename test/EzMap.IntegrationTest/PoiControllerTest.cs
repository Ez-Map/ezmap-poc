using System.Net;
using System.Net.Http.Json;
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
        var poi = new PoiCreateDto()
        {
            Name = "THANH NUMBER FAV PLACE",
            Address = "citygarden",
        };

        // act
        var response = await client.PostAsJsonAsync("poi/CreatePoi", poi);
       
        
        // assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var dbContext = scope.ServiceProvider.GetRequiredService<EzMapContext>();
        var dbPoi = dbContext.Pois.FirstOrDefault(p => p.Name == poi.Name && p.Address == poi.Address);
        
        Assert.NotNull(dbPoi);
        
    }
}