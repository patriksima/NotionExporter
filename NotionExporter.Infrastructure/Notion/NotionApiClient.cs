using System.Text;
using System.Text.Json;
using NotionExporter.Applications.Abstractions;
using NotionExporter.Infrastructure.Notion.Auth;

namespace NotionExporter.Infrastructure.Notion;

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

    public async Task<JsonDocument> RetrievePageAsync(string pageId, string? filterProperties = null)
    {
        var url = $"pages/{pageId}";

        var response = await httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var stream = await response.Content.ReadAsStreamAsync();
        return await JsonDocument.ParseAsync(stream);
    }

    public async Task<JsonDocument> RetrieveBlockChildrenAsync(string pageId, int pageSize = 100)
    {
        var url = $"blocks/{pageId}/children?page_size={pageSize}";

        var response = await httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var stream = await response.Content.ReadAsStreamAsync();
        return await JsonDocument.ParseAsync(stream);
    }
}