using System.Text.Json;

namespace NotionExporter.Infrastructure;

public interface INotionApiClient
{
    Task<JsonDocument> QueryDatabaseAsync(string databaseId, string? filterJson = null);
    Task<JsonDocument> RetrievePageAsync(string pageId, string? filterProperties = null);
    void SetToken(string apiToken);
}