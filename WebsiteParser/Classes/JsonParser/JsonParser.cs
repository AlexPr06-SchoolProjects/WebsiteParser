using System.Text.Json;
using WebsiteParser.Interfaces;

namespace WebsiteParser.Classes.JsonParser;

internal class JsonParser<TConfig> : IJsonParser<TConfig>
    where TConfig: IConfig
{
    public async Task<TConfig?> ParseFileAsync(string filePath)
    {
        var jsonContent = await File.ReadAllTextAsync(filePath);
        TConfig? data = JsonSerializer.Deserialize<TConfig>(jsonContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return data;
    }
}
