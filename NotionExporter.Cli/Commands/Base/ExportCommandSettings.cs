using System.ComponentModel;
using NotionExporter.Shared.Output;
using Spectre.Console.Cli;

namespace NotionExporter.Cli.Commands.Base;

public abstract class ExportCommandSettings : CommandSettings
{
    [CommandOption("--format <FORMAT>")]
    [Description("Output format (json or csv). Default: json")]
    public OutputFormat Format { get; set; } = OutputFormat.Json;

    [CommandOption("-o|--output <FILE>")]
    [Description("Output file")]
    public string? Output { get; set; }

    [CommandOption("-t|--token <TOKEN>")]
    [Description("API token")]
    public string? Token { get; set; }

    [CommandOption("--debug")]
    [Description("Debug")]
    public bool? Debug { get; set; }
}