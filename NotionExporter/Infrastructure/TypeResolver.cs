using Spectre.Console.Cli;

namespace NotionExporter.Infrastructure;

public sealed class TypeResolver(IServiceProvider provider) : ITypeResolver
{
    private readonly IServiceProvider _provider = provider ?? throw new ArgumentNullException(nameof(provider));

    public object? Resolve(Type? type)
    {
        return type == null ? null : _provider.GetService(type);
    }
}