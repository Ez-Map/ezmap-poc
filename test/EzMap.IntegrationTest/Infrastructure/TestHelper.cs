using System.Net.Http.Json;
using EzMap.Domain.Dtos;

namespace EzMap.IntegrationTest.Infrastructure;

static class TestHelper
{
    public static async Task<string> GetDefaultUserToken(HttpClient client)
    {
        var userSignInDto = new UserSignInDto()
        {
            Username = "stringstring",
            Password = "stringstring"
        };
        var tokenResponse = await client.PostAsJsonAsync("api/user/signin", userSignInDto);
        var token = await tokenResponse.Content.ReadAsStringAsync();
        return token;
    }

    public static string GetErrorMessageOfAProperty(Dictionary<string, string[]> errorDict, string propertyName, int index)
    {
        return errorDict[propertyName][index];
    }
}