using System.ComponentModel;
using System.Text.Json;
using NotionExporter.Helpers;
using NotionExporter.Infrastructure;
using Spectre.Console;
using Spectre.Console.Cli;

namespace NotionExporter.Commands;

public class DatabasesCommand(TokenResolver tokenResolver, INotionApiClient client)
    : JsonExportCommand<DatabasesCommand.Settings>(tokenResolver, client)
{
    public class Settings : JsonExportCommandSettings
    {
        [CommandOption("--id <DATABASE_ID>")]
        [Description("Database ID")]
        public string Id { get; set; } = default!;

        [CommandOption("--filter-json <JSON>")]
        [Description("JSON Notion query (filter + sorts)")]
        public string? FilterJson { get; set; }

        [CommandOption("--filter-file <FILE>")]
        [Description("Path to JSON file contains Notion query (filter + sorts)")]
        public string? FilterFile { get; set; }

        public override ValidationResult Validate()
        {
            return string.IsNullOrWhiteSpace(Id)
                ? ValidationResult.Error("Database ID must be specified")
                : ValidationResult.Success();
        }
    }

    protected override async Task<JsonDocument> GetJsonDocumentAsync(INotionApiClient client, Settings settings)
    {
        var queryJson = await CommandHelper.ResolveQueryJson(settings.FilterJson, settings.FilterFile);
        return await client.QueryDatabaseAsync(settings.Id, queryJson);
    }
}