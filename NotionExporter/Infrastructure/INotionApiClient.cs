using System.Text.Json;

namespace NotionExporter.Infrastructure;

public interface INotionApiClient
{
    Task<JsonDocument> QueryDatabaseAsync(string databaseId, string? filterJson = null);
    void SetToken(string apiToken);
}