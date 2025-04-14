using System.Text;
using System.Text.Json;

namespace NotionExporter.Infrastructure;

public class NotionApiClient(HttpClient httpClient, NotionAuthHandler authHandler) : INotionApiClient
{
    public void SetToken(string token)
    {
        authHandler.SetToken(token);
    }
    
    public async Task<JsonDocument> QueryDatabaseAsync(string databaseId, string? filterJson = null)
    {
        var url = $"databases/{databaseId}/query";

        var payload = string.IsNullOrWhiteSpace(filterJson)
            ? "{}"
            : filterJson;

        var content = new StringContent(payload, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync(url, content);
        response.EnsureSuccessStatusCode();

        var stream = await response.Content.ReadAsStreamAsync();
        return await JsonDocument.ParseAsync(stream);
    }
}