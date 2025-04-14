using Microsoft.Extensions.Options;
using Spectre.Console;

namespace NotionExporter.Infrastructure;

public class TokenResolver(IOptions<NotionSettings> options)
{
    private readonly NotionSettings _settings = options.Value;
    
    public string ResolveToken(string? cliToken)
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