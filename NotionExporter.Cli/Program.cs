using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NotionExporter.Applications.Abstractions;
using NotionExporter.Applications.Handlers;
using NotionExporter.Applications.Requests;
using NotionExporter.Cli.Commands;
using NotionExporter.Cli.Spectre;
using NotionExporter.Infrastructure.Config;
using NotionExporter.Infrastructure.Notion;
using NotionExporter.Infrastructure.Notion.Auth;
using NotionExporter.Infrastructure.Output;
using NotionExporter.Shared.Config;
using Spectre.Console.Cli;

var services = new ServiceCollection();

var configRoot = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

// settings
services.Configure<NotionSettings>(configRoot.GetSection("Notion"));
services.AddSingleton<IConfiguration>(configRoot);

// spectre commands
services.AddSingleton<ExportDatabaseCommand>();
services.AddSingleton<ExportPagesCommand>();
services.AddSingleton<ExportBlocksCommand>();

// output writers
services.AddSingleton<IOutputWriter, JsonFileWriter>();
services.AddSingleton<IOutputWriter, CsvFileWriter>();
services.AddSingleton<IOutputWriterFactory, OutputWriterFactory>();
services.AddSingleton<IStreamProvider, StreamProvider>();

// handlers
services.AddTransient<IHandler<ExportDatabasesRequest>, ExportDatabasesHandler>();
services.AddTransient<IHandler<ExportPagesRequest>, ExportPagesHandler>();
services.AddTransient<IHandler<ExportBlocksRequest>, ExportBlocksHandler>();

// Notion client
services.AddSingleton<NotionAuthHandler>();
services.AddSingleton<ITokenResolver, TokenResolver>();

services.AddHttpClient<INotionApiClient, NotionApiClient>((sp, client) =>
    {
        var settings = sp.GetRequiredService<IOptions<NotionSettings>>().Value;

        client.BaseAddress = new Uri(settings.BaseUrl);
        client.DefaultRequestHeaders.Add("Notion-Version", settings.ApiVersion);
        client.DefaultRequestHeaders.Add("Accept", "application/json");

        if (!string.IsNullOrWhiteSpace(settings.ApiToken))
        {
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", settings.ApiToken);
        }
    })
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler())
    .AddHttpMessageHandler<NotionAuthHandler>();

var registrar = new TypeRegistrar(services);
var app = new CommandApp(registrar);

app.Configure(config =>
{
#if DEBUG
    config.PropagateExceptions();
#endif
    config.SetApplicationName("notion-exporter");
    config.ValidateExamples();

    config.AddExample("databases", "--id", "123456756643812e81aad451290be2aa");
    config.AddExample("databases", "--id", "123456756643812e81aad451290be2aa", "--output", "output.json");
    config.AddExample("pages", "--id", "123456756643812e81aad451290be2aa");
    config.AddExample("pages", "--id", "123456756643812e81aad451290be2aa", "--output", "output.json");
    config.AddExample("blocks", "--id", "123456756643812e81aad451290be2aa");
    config.AddExample("blocks", "--id", "123456756643812e81aad451290be2aa", "--output", "output.json");

    config.AddCommand<ExportDatabaseCommand>("databases");
    config.AddCommand<ExportPagesCommand>("pages");
    config.AddCommand<ExportBlocksCommand>("blocks");
});

return app.Run(args);