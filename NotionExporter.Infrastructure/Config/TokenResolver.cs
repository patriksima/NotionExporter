using Microsoft.Extensions.Options;
using NotionExporter.Applications.Abstractions;
using NotionExporter.Shared.Config;

namespace NotionExporter.Infrastructure.Config;

public class TokenResolver(IOptions<NotionSettings> options) : ITokenResolver
{
    private readonly NotionSettings _settings = options.Value;

    public string? ResolveToken(string? cliToken)
    {
        if (!string.IsNullOrWhiteSpace(cliToken))
            return cliToken;

        var configToken = _settings.ApiToken;
        if (!string.IsNullOrWhiteSpace(configToken))
            return configToken;

        var envToken = Environment.GetEnvironmentVariable("NOTION_API_TOKEN");
        if (!string.IsNullOrWhiteSpace(envToken))
            return envToken;

        return null;
    }
}