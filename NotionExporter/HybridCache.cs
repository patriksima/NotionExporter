using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;

namespace NotionExporter;

public interface ICache
{
    Task<T?> GetOrAddAsync<T>(string key, Func<Task<T>> factory, TimeSpan? ttl = null);
}

public class HybridCache : ICache
{
    private readonly MemoryCache _memory = new(new MemoryCacheOptions());
    private readonly string _cacheDir;

    public HybridCache(string cacheDir = "cache")
    {
        _cacheDir = cacheDir;
        Directory.CreateDirectory(_cacheDir);
    }

    public async Task<T?> GetOrAddAsync<T>(string key, Func<Task<T>> factory, TimeSpan? ttl = null)
    {
        if (_memory.TryGetValue<T>(key, out var cachedInMemory))
            return cachedInMemory;

        var path = Path.Combine(_cacheDir, $"{SanitizeKey(key)}.json");

        if (File.Exists(path))
        {
            var fileInfo = new FileInfo(path);
            if (!ttl.HasValue || fileInfo.LastWriteTime > DateTime.Now - ttl.Value)
            {
                var json = await File.ReadAllTextAsync(path);
                var value = JsonSerializer.Deserialize<T>(json);
                if (value is not null)
                {
                    _memory.Set(key, value, ttl ?? TimeSpan.FromMinutes(10));
                    return value;
                }
            }
        }

        var newValue = await factory();
        _memory.Set(key, newValue, ttl ?? TimeSpan.FromMinutes(10));

        var serialized = JsonSerializer.Serialize(newValue, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(path, serialized);

        return newValue;
    }

    private static string SanitizeKey(string key)
    {
        return Path.GetInvalidFileNameChars().Aggregate(key, (current, c) => current.Replace(c, '_'));
    }
}