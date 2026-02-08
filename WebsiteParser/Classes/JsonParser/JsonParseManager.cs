using WebsiteParser.Interfaces;
using WebsiteParser.Records;

namespace WebsiteParser.Classes.JsonParser;

internal class JsonParseManager<TConfig>(IJsonParser<TConfig> jsonParser)
    where TConfig : class, IConfig
{
    public async Task<JsonParseResult<TConfig>> JsonParseResult (string filePath)
    {
        try
        {
            TConfig? parsedJson = await jsonParser.ParseFileAsync(filePath);
            if (parsedJson is null)
            {
                return new JsonParseError<TConfig>($"File {filePath} was empty or not found.");
            }
            return new JsonParseSuccess<TConfig>(parsedJson, "Data parsed successfully");
        }
        catch (Exception ex)
        {
            return new JsonParseWarning<TConfig>($"Parsing failed: {ex.Message}");
        }
    }
}
