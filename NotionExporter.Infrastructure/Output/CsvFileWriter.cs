using System.Text;
using System.Text.Json;
using NotionExporter.Applications.Abstractions;
using NotionExporter.Shared.Output;

namespace NotionExporter.Infrastructure.Output;

public class CsvFileWriter : IOutputWriter
{
    public OutputFormat Format => OutputFormat.Csv;

    public void Write(JsonDocument document, string? outputPath)
    {
        if (document.RootElement.GetProperty("results").ValueKind != JsonValueKind.Array)
            throw new InvalidOperationException("Invalid data for CSV export.");

        var results = document.RootElement.GetProperty("results").EnumerateArray().ToList();

        if (results.Count == 0)
            return;

        var first = results.First();
        var headers = first.GetProperty("properties").EnumerateObject().Select(p => p.Name).ToList();

        using var writer = string.IsNullOrWhiteSpace(outputPath)
            ? new StreamWriter(Console.OpenStandardOutput(), new UTF8Encoding(false)) { AutoFlush = true }
            : new StreamWriter(outputPath, false, new UTF8Encoding(false)) { AutoFlush = true };
        writer.WriteLine(string.Join(";", headers.Select(Escape)));

        foreach (var item in results)
        {
            var row = new List<string>();
            var props = item.GetProperty("properties");

            foreach (var header in headers)
            {
                if (props.TryGetProperty(header, out var prop))
                {
                    var value = ExtractValue(prop);
                    row.Add(Escape(value));
                }
                else
                {
                    row.Add("");
                }
            }

            writer.WriteLine(string.Join(";", row));
        }
    }

    private static string Escape(string value)
    {
        //if (value.Contains(',') || value.Contains('"'))
            return $"\"{value.Replace("\"", "\"\"")}\"";
        //return value;
    }

    private static string ExtractValue(JsonElement prop)
    {
        if (prop.TryGetProperty("title", out var title) && title.ValueKind == JsonValueKind.Array)
            return string.Join(" ", title.EnumerateArray().Select(t => t.GetProperty("plain_text").GetString()));

        if (prop.TryGetProperty("rich_text", out var rt) && rt.ValueKind == JsonValueKind.Array)
            return string.Join(" ", rt.EnumerateArray().Select(t => t.GetProperty("plain_text").GetString()));

        if (prop.TryGetProperty("select", out var sel) && sel.TryGetProperty("name", out var name))
            return name.GetString() ?? string.Empty;

        if (prop.TryGetProperty("multi_select", out var ms) && ms.ValueKind == JsonValueKind.Array)
            return string.Join(",", ms.EnumerateArray().Select(t => t.GetProperty("name").GetString()));

        if (prop.TryGetProperty("date", out var date) && date.TryGetProperty("start", out var start))
            return start.GetString() ?? string.Empty;
        
        if (prop.TryGetProperty("status", out var status) && status.TryGetProperty("name", out var statusName))
            return statusName.GetString() ?? string.Empty;
        
        if (prop.TryGetProperty("unique_id", out var uniqueId) && uniqueId.TryGetProperty("number", out var uniqueIdNumber))
            return uniqueIdNumber.GetRawText();

        if (prop.TryGetProperty("number", out var number))
            return number.GetRawText();
        
        if (prop.ValueKind == JsonValueKind.String)
            return prop.GetString() ?? string.Empty;

        return prop.ToString();
    }
}