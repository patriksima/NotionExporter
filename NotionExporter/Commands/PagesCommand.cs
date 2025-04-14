using System.ComponentModel;
using System.Text.Json;
using Microsoft.Extensions.Options;
using NotionExporter.Infrastructure;
using NotionExporter.Options;
using Spectre.Console;
using Spectre.Console.Cli;

namespace NotionExporter.Commands;

public class PagesCommand(TokenResolver tokenResolver, INotionApiClient client)
    : AsyncCommand<PagesCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandArgument(0, "[OPERATION]")]
        [Description("Operation to execute. Default: export")]
        [TypeConverter(typeof(OperationConverter))]
        public Operation? Operation { get; set; } = Options.Operation.Export;

        [CommandOption("--id <PAGE_ID>")]
        [Description("Page ID")]
        public string Id { get; set; } = default!;

        [CommandOption("-o|--output <FILE>")]
        [Description("Output file")]
        public string? Output { get; set; }

        [CommandOption("-t|--token <TOKEN>")]
        [Description("API token")]
        public string? Token { get; set; }

        [CommandOption("--debug")]
        [Description("Debug")]
        public bool? Debug { get; set; }

        public override ValidationResult Validate()
        {
            return string.IsNullOrWhiteSpace(Id)
                ? ValidationResult.Error("Page ID must be specified")
                : ValidationResult.Success();
        }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var apiToken = tokenResolver.ResolveToken(settings.Token);

        client.SetToken(apiToken);

        var jsonDoc = await client.RetrievePageAsync(settings.Id);

        var fileName = settings.Output ?? $"page-{settings.Id}.json";
        await using (var fs = File.Create(fileName))
        {
            await using var writer = new Utf8JsonWriter(fs, new JsonWriterOptions
            {
                Indented = true
            });

            jsonDoc.WriteTo(writer);
        }

        AnsiConsole.MarkupLine($"[green]Exported to file:[/] [blue]{fileName}[/]");

        return 0;
    }
}