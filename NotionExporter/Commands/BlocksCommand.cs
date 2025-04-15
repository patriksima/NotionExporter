using System.ComponentModel;
using System.Text.Json;
using NotionExporter.Infrastructure;
using Spectre.Console.Cli;

namespace NotionExporter.Commands;

public class BlocksCommand(TokenResolver tokenResolver, INotionApiClient client)
    : JsonExportCommand<BlocksCommand.Settings>(tokenResolver, client)
{
    public class Settings : JsonExportCommandSettings
    {
        [CommandOption("--id <PAGE_ID>")]
        [Description("Page ID")]
        public string Id { get; set; } = default!;
    }

    protected override Task<JsonDocument> GetJsonDocumentAsync(INotionApiClient client, Settings settings)
    {
        return client.RetrieveBlockChildrenAsync(settings.Id);
    }
}