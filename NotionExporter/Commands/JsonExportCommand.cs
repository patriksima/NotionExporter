using System.Text.Json;
using NotionExporter.Helpers;
using NotionExporter.Infrastructure;
using Spectre.Console.Cli;

namespace NotionExporter.Commands;

public abstract class JsonExportCommand<TSettings>(TokenResolver tokenResolver, INotionApiClient client)
    : AsyncCommand<TSettings> where TSettings : JsonExportCommandSettings
{
    public override async Task<int> ExecuteAsync(CommandContext context, TSettings settings)
    {
        var apiToken = tokenResolver.ResolveToken(settings.Token);
        client.SetToken(apiToken);

        var jsonDoc = await GetJsonDocumentAsync(client, settings);
        await CommandHelper.OutputJsonAsync(jsonDoc, settings.Output);
        return 0;
    }

    protected abstract Task<JsonDocument> GetJsonDocumentAsync(INotionApiClient client, TSettings settings);
}