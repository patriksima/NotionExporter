using System.Text.Json;
using Moq;
using NotionExporter.Applications.Abstractions;
using NotionExporter.Applications.Handlers;
using NotionExporter.Applications.Requests;
using NotionExporter.Infrastructure.Output;
using NotionExporter.Shared.Output;

namespace NotionExporter.Tests;

public class ExportDatabasesTest
{
    [Fact]
    public async Task Exports_Database_To_Csv_Successfully()
    {
        // Arrange
        var mockClient = new Mock<INotionApiClient>();
        var testStream = new TestOutputStreamProvider();

        var json = """
                   {
                       "results": [
                           {
                               "properties": {
                                   "Name": { "title": [ { "plain_text": "Test Page" } ] },
                                   "Status": { "status": { "name": "In Progress" } }
                               }
                           }
                       ]
                   }
                   """;

        var fakeJson = JsonDocument.Parse(json);
        mockClient.Setup(c => c.QueryDatabaseAsync("abc", null)).ReturnsAsync(fakeJson);

        var writer = new CsvFileWriter(testStream);
        var factory = new Mock<IOutputWriterFactory>();
        factory.Setup(f => f.GetWriter(OutputFormat.Csv)).Returns(writer);

        var handler = new ExportDatabasesHandler(mockClient.Object, factory.Object);

        var request = new ExportDatabasesRequest
        {
            Id = "abc",
            Format = OutputFormat.Csv,
            Token = "test-token",
            Output = null // null = write to stdout = our test stream
        };

        // Act
        await handler.ExecuteAsync(request);

        // Assert
        mockClient.Verify(c => c.SetToken("test-token"), Times.Once);
        var output = testStream.GetCapturedText();

        Assert.Contains("\"Name\";\"Status\"", output);
        Assert.Contains("\"Test Page\";\"In Progress\"", output);
    }

    [Fact]
    public async Task Exports_Database_To_Json_Successfully()
    {
        // Arrange
        var mockClient = new Mock<INotionApiClient>();
        var testStream = new TestOutputStreamProvider();

        var json = """
                   {
                       "results": [
                           {
                               "properties": {
                                   "Name": { "title": [ { "plain_text": "Test Page" } ] },
                                   "Status": { "status": { "name": "In Progress" } }
                               }
                           }
                       ]
                   }
                   """;

        var fakeJson = JsonDocument.Parse(json);
        mockClient.Setup(c => c.QueryDatabaseAsync("abc", null)).ReturnsAsync(fakeJson);

        var writer = new JsonFileWriter(testStream);
        var factory = new Mock<IOutputWriterFactory>();
        factory.Setup(f => f.GetWriter(OutputFormat.Json)).Returns(writer);

        var handler = new ExportDatabasesHandler(mockClient.Object, factory.Object);

        var request = new ExportDatabasesRequest
        {
            Id = "abc",
            Format = OutputFormat.Json,
            Token = "test-token",
            Output = null // null = write to stdout = our test stream
        };

        // Act
        await handler.ExecuteAsync(request);

        // Assert
        mockClient.Verify(c => c.SetToken("test-token"), Times.Once);
        var output = testStream.GetCapturedText();

        var doc = JsonDocument.Parse(output);

        var name = doc.RootElement
            .GetProperty("results")[0]
            .GetProperty("properties")
            .GetProperty("Name")
            .GetProperty("title")[0]
            .GetProperty("plain_text")
            .GetString();

        Assert.Equal("Test Page", name);
    }
}