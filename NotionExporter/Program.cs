using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NotionExporter;
using NotionExporter.Commands;
using NotionExporter.Infrastructure;
using Spectre.Console.Cli;

var services = new ServiceCollection();

var configRoot = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

services.Configure<NotionSettings>(configRoot.GetSection("Notion"));
services.AddSingleton<IConfiguration>(configRoot);
services.AddSingleton<DatabasesCommand>();
services.AddSingleton<PagesCommand>();
services.AddSingleton<BlocksCommand>();
services.AddSingleton<NotionAuthHandler>();
services.AddSingleton<TokenResolver>();

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
;

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
    
    config.AddCommand<DatabasesCommand>("databases");
    config.AddCommand<PagesCommand>("pages");
    config.AddCommand<BlocksCommand>("blocks");
});

return app.Run(args);