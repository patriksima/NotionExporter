using System.ComponentModel;
using System.Text.Json;
using NotionExporter.Infrastructure;
using Spectre.Console;
using Spectre.Console.Cli;

namespace NotionExporter.Commands;

public class PagesCommand(TokenResolver tokenResolver, INotionApiClient client)
    : JsonExportCommand<PagesCommand.Settings>(tokenResolver, client)
{
    public class Settings : JsonExportCommandSettings
    {
        [CommandOption("--id <PAGE_ID>")]
        [Description("Page ID")]
        public string Id { get; set; } = default!;

        public override ValidationResult Validate()
        {
            return string.IsNullOrWhiteSpace(Id)
                ? ValidationResult.Error("Page ID must be specified")
                : ValidationResult.Success();
        }
    }

    protected override Task<JsonDocument> GetJsonDocumentAsync(INotionApiClient client, Settings settings)
    {
        return client.RetrievePageAsync(settings.Id);
    }
}