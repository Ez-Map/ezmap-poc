using System.Net.Http.Headers;
using System.Text.Json;

namespace EzMap.IntegrationTest.Infrastructure;

public static class HttpHelper
{
    public static Task<HttpResponseMessage> 
        RequestAsJsonAsyncWithToken<T>(this HttpClient client, HttpMethod method,
        string uri,
        string token, T? dto = default)
    {
        HttpRequestMessage request = new HttpRequestMessage(method, uri);

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        if (dto is not null)
        {
            request.Content = new StringContent(JsonSerializer.Serialize(dto, new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }), System.Text.Encoding.UTF8, "application/json");
        }

        return client.SendAsync(request);
    }
}