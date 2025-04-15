using System.ComponentModel;
using Spectre.Console.Cli;

namespace NotionExporter.Commands;

public abstract class JsonExportCommandSettings : CommandSettings
{
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