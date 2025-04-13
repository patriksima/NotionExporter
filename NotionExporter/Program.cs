using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotionExporter;
using HybridCache = NotionExporter.HybridCache;

var services = new ServiceCollection();

services.AddSingleton<ICache, HybridCache>();
services.AddSingleton<IConfiguration>(new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true)
    .Build());

var serviceProvider = services.BuildServiceProvider();
var cache = serviceProvider.GetService<ICache>();
var config = serviceProvider.GetService<IConfiguration>();

var notionApiToken = config["Notion:ApiToken"];
if (string.IsNullOrEmpty(notionApiToken))
{
    Console.WriteLine("Missing Notion API token in appsettings.json.");
    return;
}

using var client = new HttpClient();
client.BaseAddress = new Uri(config["Notion:BaseUrl"]);
client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", notionApiToken);
client.DefaultRequestHeaders.Add("Notion-Version", "2022-06-28");

var databaseId = config["Notion:DatabaseId"];
const string queryJson = """
                         {
                             "filter": {
                                 "property": "Date",
                                 "date": {
                                     "this_week": {}
                                 }
                             },
                             "sorts": [
                                 {
                                     "property": "Name",
                                     "direction": "ascending"
                                 }
                             ]
                         }
                         """;

var content = new StringContent(queryJson, Encoding.UTF8, "application/json");

// 4. Dotaz na databázi
var json = await cache.GetOrAddAsync(
    $"notion:database:{databaseId}",
    async () =>
    {
        var response = await client.PostAsync($"databases/{databaseId}/query", content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    },
    TimeSpan.FromMinutes(60)
);

var doc = JsonDocument.Parse(json, new JsonDocumentOptions { AllowTrailingCommas = true });

var pageIds = new List<string>();

foreach (var result in doc.RootElement.GetProperty("results").EnumerateArray())
{
    if (result.TryGetProperty("object", out var objType) && objType.GetString() == "page")
    {
        if (result.TryGetProperty("id", out var idProp))
        {
            var id = idProp.GetString();
            if (!string.IsNullOrEmpty(id))
            {
                pageIds.Add(id);
            }
        }
    }
}


foreach (var pageId in pageIds)
{
    var jsonBlock = await cache.GetOrAddAsync(
        $"notion:blocks:{pageId}",
        async () =>
        {
            var response = await client.GetAsync($"blocks/{pageId}/children?page_size=100");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        },
        TimeSpan.FromMinutes(60)
    );
    
    var blockDoc = JsonDocument.Parse(jsonBlock);

    foreach (var result in blockDoc.RootElement.GetProperty("results").EnumerateArray())
    {
        var plainText = ExtractTextFromBlock(result);
        Console.WriteLine(plainText);
    }
}

return;

/*
Console.WriteLine($"Načteno {blockIds.Count} bloků");

foreach (var blockId in blockIds)
{
    var blockResponse = await client.GetAsync($"blocks/{blockId}");
    blockResponse.EnsureSuccessStatusCode();

    await using var blockStream = await blockResponse.Content.ReadAsStreamAsync();
    using var blockDoc = await JsonDocument.ParseAsync(blockStream);

    var plainText = ExtractTextFromBlock(blockDoc.RootElement);
    Console.WriteLine(plainText);
}
*/
string? ExtractTextFromBlock(JsonElement block)
{
    if (!block.TryGetProperty("type", out var typeProp))
        return null;

    var type = typeProp.GetString();

    if (string.IsNullOrEmpty(type))
        return null;

    if (!block.TryGetProperty(type, out var typedContent))
        return null;

    if (!typedContent.TryGetProperty("rich_text", out var richTextArray))
        return null;

    var text = new StringBuilder();

    foreach (var rt in richTextArray.EnumerateArray())
    {
        if (rt.TryGetProperty("plain_text", out var plainText))
        {
            text.Append(plainText.GetString());
        }
    }

    return text.ToString();
}