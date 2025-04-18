namespace NotionExporter.Applications.Abstractions;

public interface ITokenResolver
{
    string? ResolveToken(string? cliToken);
}