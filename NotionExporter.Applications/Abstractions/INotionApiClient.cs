using System.Text.Json;

namespace NotionExporter.Applications.Abstractions;

public interface INotionApiClient
{
    Task<JsonDocument> ListDatabasesAsync();
    Task<JsonDocument> QueryDatabaseAsync(string databaseId, string? filterJson = null);
    Task<JsonDocument> RetrievePageAsync(string pageId, string? filterProperties = null);
    Task<JsonDocument> RetrieveBlockChildrenAsync(string pageId, int pageSize = 100);
    void SetToken(string apiToken);
}