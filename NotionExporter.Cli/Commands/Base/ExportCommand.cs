using NotionExporter.Applications.Abstractions;
using Spectre.Console;
using Spectre.Console.Cli;


namespace NotionExporter.Cli.Commands.Base;

public abstract class ExportCommand<TSettings, TRequest>(
    IHandler<TRequest> handler,
    ITokenResolver tokenResolver) : AsyncCommand<TSettings> where TSettings : ExportCommandSettings
{
    public override async Task<int> ExecuteAsync(CommandContext context, TSettings settings)
    {
        var apiToken = tokenResolver.ResolveToken(settings.Token);

        if (string.IsNullOrEmpty(apiToken))
        {
            apiToken = AnsiConsole.Prompt(
                new TextPrompt<string>("Input Notion API Token:")
                    .PromptStyle("red")
                    .Secret());
        }

        await handler.ExecuteAsync(MapRequest(settings, apiToken));

        return 0;
    }

    protected abstract TRequest MapRequest(TSettings settings, string apiToken);
}