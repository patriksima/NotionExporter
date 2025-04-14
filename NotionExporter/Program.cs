using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NotionExporter;
using NotionExporter.Commands;
using NotionExporter.Infrastructure;
using Spectre.Console.Cli;

var services = new ServiceCollection();

var configRoot = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

services.Configure<NotionSettings>(configRoot.GetSection("Notion"));
services.AddSingleton<IConfiguration>(configRoot);
services.AddSingleton<DatabasesCommand>();
services.AddSingleton<PagesCommand>();
services.AddSingleton<NotionAuthHandler>();
services.AddSingleton<TokenResolver>();

services.AddHttpClient<INotionApiClient, NotionApiClient>((sp, client) =>
    {
        var settings = sp.GetRequiredService<IOptions<NotionSettings>>().Value;

        client.BaseAddress = new Uri(settings.BaseUrl);
        client.DefaultRequestHeaders.Add("Notion-Version", "2022-06-28");
        client.DefaultRequestHeaders.Add("Accept", "application/json");

        if (!string.IsNullOrWhiteSpace(settings.ApiToken))
        {
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", settings.ApiToken);
        }
    })
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler())
    .AddHttpMessageHandler<NotionAuthHandler>();
;

var registrar = new TypeRegistrar(services);
var app = new CommandApp(registrar);

app.Configure(config =>
{
    config.PropagateExceptions();
    config.SetApplicationName("notion-exporter");
    config.ValidateExamples();

   // config.AddExample("databases", "--id", "1c5e79756643812e81c2d451290be2cf", "--output", "output.json");

   config.AddCommand<DatabasesCommand>("databases");
   config.AddCommand<PagesCommand>("pages");
});

return app.Run(args);

/*
var services = ConfigureServices();
await using var serviceProvider = services.BuildServiceProvider();

var config = serviceProvider.GetRequiredService<IConfiguration>();
var cache = serviceProvider.GetRequiredService<ICache>();
var client = CreateHttpClient(config);

var databaseId = config["Notion:DatabaseId"]
                 ?? throw new InvalidOperationException("Missing Notion:DatabaseId in configuration.");

var queryContent = CreateQueryContent();

var responseJson = await cache.GetOrAddAsync(
    $"notion:database:{databaseId}",
    async () => await QueryDatabaseAsync(client, databaseId, queryContent),
    TimeSpan.FromMinutes(60)
);

var pageIds = ExtractPageIds(responseJson);

var outputFile = "notion-output.txt";
await using var writer = new StreamWriter(outputFile, append: false, encoding: Encoding.UTF8);

foreach (var pageId in pageIds)
{
    var blockJson = await cache.GetOrAddAsync(
        $"notion:blocks:{pageId}",
        async () => await FetchPageBlocksAsync(client, pageId),
        TimeSpan.FromMinutes(60)
    );

    foreach (var text in ExtractTextsFromBlocks(blockJson))
    {
        Console.WriteLine(text);
        await writer.WriteLineAsync(text);
    }
}

writer.Close();


// -------------------- Helpers --------------------

static IServiceCollection ConfigureServices()
{
    return new ServiceCollection()
        .AddSingleton<ICache, HybridCache>()
        .AddSingleton<IConfiguration>(provider =>
            new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true)
                .Build()
        );
}

static HttpClient CreateHttpClient(IConfiguration config)
{
    var token = config["Notion:ApiToken"]
                ?? throw new InvalidOperationException("Missing Notion:ApiToken in configuration.");

    var baseUrl = config["Notion:BaseUrl"]
                  ?? throw new InvalidOperationException("Missing Notion:BaseUrl in configuration.");

    var client = new HttpClient
    {
        BaseAddress = new Uri(baseUrl)
    };

    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    client.DefaultRequestHeaders.Add("Notion-Version", "2022-06-28");

    return client;
}

static StringContent CreateQueryContent()
{
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

    return new StringContent(queryJson, Encoding.UTF8, "application/json");
}

static async Task<string> QueryDatabaseAsync(HttpClient client, string databaseId, HttpContent content)
{
    var response = await client.PostAsync($"databases/{databaseId}/query", content);
    response.EnsureSuccessStatusCode();
    return await response.Content.ReadAsStringAsync();
}

static async Task<string> FetchPageBlocksAsync(HttpClient client, string pageId)
{
    var response = await client.GetAsync($"blocks/{pageId}/children?page_size=100");
    response.EnsureSuccessStatusCode();
    return await response.Content.ReadAsStringAsync();
}

static List<string> ExtractPageIds(string json)
{
    var pageIds = new List<string>();
    using var doc = JsonDocument.Parse(json, new JsonDocumentOptions { AllowTrailingCommas = true });

    foreach (var result in doc.RootElement.GetProperty("results").EnumerateArray())
    {
        if (result.TryGetProperty("object", out var objType) && objType.GetString() == "page" &&
            result.TryGetProperty("id", out var idProp))
        {
            var id = idProp.GetString();
            if (!string.IsNullOrWhiteSpace(id))
            {
                pageIds.Add(id);
            }
        }
    }

    return pageIds;
}

static IEnumerable<string> ExtractTextsFromBlocks(string json)
{
    using var doc = JsonDocument.Parse(json);

    foreach (var block in doc.RootElement.GetProperty("results").EnumerateArray())
    {
        var text = ExtractTextFromBlock(block);
        if (!string.IsNullOrEmpty(text))
        {
            yield return text;
        }
    }
}

static string? ExtractTextFromBlock(JsonElement block)
{
    if (!block.TryGetProperty("type", out var typeProp)) return null;

    var type = typeProp.GetString();
    if (string.IsNullOrEmpty(type)) return null;

    if (!block.TryGetProperty(type, out var typedContent)) return null;
    if (!typedContent.TryGetProperty("rich_text", out var richTextArray)) return null;

    var builder = new StringBuilder();
    foreach (var rt in richTextArray.EnumerateArray())
    {
        if (rt.TryGetProperty("plain_text", out var plainText))
        {
            builder.Append(plainText.GetString());
        }
    }

    return builder.ToString();
}
*/