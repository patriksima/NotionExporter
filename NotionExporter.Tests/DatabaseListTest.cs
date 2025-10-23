using System.Text.Json;
using Moq;
using NotionExporter.Applications.Abstractions;
using NotionExporter.Applications.Handlers;
using NotionExporter.Applications.Requests;
using NotionExporter.Infrastructure.Output;
using NotionExporter.Shared.Output;

namespace NotionExporter.Tests;

public class DatabaseListTest
{
    [Fact]
    public async Task Lists_Databases_To_Json_Successfully()
    {
        // Arrange
        var mockClient = new Mock<INotionApiClient>();
        var testStream = new TestOutputStreamProvider();

        var json = """
                   {
                       "object": "list",
                       "results": [
                           { "object": "database", "id": "db1", "title": [ { "plain_text": "Demo DB 1" } ], "href": "https://example.com/db1" },
                           { "object": "database", "id": "db2", "title": [ { "plain_text": "Demo DB 2" } ], "href": "https://example.com/db2" }
                       ]
                   }
                   """;

        var fakeJson = JsonDocument.Parse(json);
        mockClient.Setup(c => c.ListDatabasesAsync()).ReturnsAsync(fakeJson);

        var writer = new JsonFileWriter(testStream);
        var factory = new Mock<IOutputWriterFactory>();
        factory.Setup(f => f.GetWriter(OutputFormat.Json)).Returns(writer);

        var handler = new DatabaseListHandler(mockClient.Object, factory.Object);

        var request = new DatabaseListRequest
        {
            Token = "test-token",
            Format = OutputFormat.Json,
            Output = null, // stdout -> test stream
            Debug = null
        };

        // Act
        await handler.ExecuteAsync(request);

        // Assert
        mockClient.Verify(c => c.SetToken("test-token"), Times.Once);

        var output = testStream.GetCapturedText();
        var doc = JsonDocument.Parse(output);
        var results = doc.RootElement.GetProperty("results");
        Assert.Equal(2, results.GetArrayLength());
        var firstObject = results[0].GetProperty("object").GetString();
        Assert.Equal("database", firstObject);
        var firstId = results[0].GetProperty("id").GetString();
        Assert.Equal("db1", firstId);
    }
}
