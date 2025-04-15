using System.Text.Json;
using NotionExporter.Infrastructure;

namespace NotionExporter.Helpers;

public static class CommandHelper
{
    public static string ResolveToken(TokenResolver tokenResolver, string? token)
    {
        return tokenResolver.ResolveToken(token);
    }

    public static async Task<string?> ResolveQueryJson(string? json, string? filePath)
    {
        if (!string.IsNullOrWhiteSpace(json))
            return json;

        if (!string.IsNullOrWhiteSpace(filePath))
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File '{filePath}' not found.");

            return await File.ReadAllTextAsync(filePath);
        }

        return null;
    }

    public static async Task OutputJsonAsync(JsonDocument jsonDoc, string? outputPath)
    {
        if (string.IsNullOrWhiteSpace(outputPath))
        {
            Console.WriteLine(JsonSerializer.Serialize(jsonDoc, new JsonSerializerOptions
            {
                WriteIndented = true
            }));
        }
        else
        {
            await using var fs = File.Create(outputPath);
            await using var writer = new Utf8JsonWriter(fs, new JsonWriterOptions
            {
                Indented = true
            });

            jsonDoc.WriteTo(writer);
        }
    }
}
