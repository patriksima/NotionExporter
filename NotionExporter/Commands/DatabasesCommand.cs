using System.ComponentModel;
using System.Text.Json;
using Microsoft.Extensions.Options;
using NotionExporter.Infrastructure;
using Spectre.Console;
using Spectre.Console.Cli;

namespace NotionExporter.Commands;

public class DatabasesCommand(IOptions<NotionSettings> options, INotionApiClient client)
    : AsyncCommand<DatabasesCommand.Settings>
{
    private readonly NotionSettings _settings = options.Value;

    public class Settings : CommandSettings
    {
        [CommandArgument(0, "[OPERATION]")]
        [Description("Operation to execute (default: export)")]
        public string? Operation { get; set; } = "export";

        [CommandOption("--id <DATABASE_ID>")]
        [Description("Database ID")]
        public string Id { get; set; } = default!;

        [CommandOption("--filter-json <JSON>")]
        [Description("JSON Notion query (filter + sorts)")]
        public string? FilterJson { get; set; }

        [CommandOption("--filter-file <FILE>")]
        [Description("Path to JSON file contains Notion query (filter + sorts)")]
        public string? FilterFile { get; set; }

        [CommandOption("-o|--output <FILE>")]
        [Description("Output file")]
        public string Output { get; set; } = "databases.json";

        [CommandOption("-t|--token <TOKEN>")]
        [Description("API token")]
        public string? Token { get; set; }

        [CommandOption("--debug")]
        [Description("Debug")]
        public bool? Debug { get; set; }

        public override ValidationResult Validate()
        {
            return string.IsNullOrWhiteSpace(Id)
                ? ValidationResult.Error("Database ID must be specified")
                : ValidationResult.Success();
        }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var apiToken = ResolveToken(settings.Token);

        client.SetToken(apiToken);

        var queryJson = settings.FilterJson;

        if (string.IsNullOrWhiteSpace(queryJson) && !string.IsNullOrWhiteSpace(settings.FilterFile))
        {
            if (!File.Exists(settings.FilterFile))
                throw new FileNotFoundException();

            queryJson = await File.ReadAllTextAsync(settings.FilterFile);
        }

        var jsonDoc = await client.QueryDatabaseAsync(settings.Id, queryJson);

        await using (var fs = File.Create(settings.Output))
        {
            await using var writer = new Utf8JsonWriter(fs, new JsonWriterOptions
            {
                Indented = true
            });

            jsonDoc.WriteTo(writer);
        }

        AnsiConsole.MarkupLine($"[green]Exported to file:[/] [blue]{settings.Output}[/]");

        return 0;
    }

    private string ResolveToken(string? cliToken)
    {
        if (!string.IsNullOrWhiteSpace(cliToken))
            return cliToken;

        var configToken = _settings.ApiToken;
        if (!string.IsNullOrWhiteSpace(configToken))
            return configToken;

        var envToken = Environment.GetEnvironmentVariable("NOTION_API_TOKEN");
        if (!string.IsNullOrWhiteSpace(envToken))
            return envToken;

        return AnsiConsole.Prompt(
            new TextPrompt<string>("Input Notion API Token:")
                .PromptStyle("red")
                .Secret());
    }
}