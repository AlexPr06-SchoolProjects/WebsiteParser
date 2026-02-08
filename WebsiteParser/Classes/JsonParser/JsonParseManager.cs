using WebsiteParser.Interfaces;
using WebsiteParser.Records;

namespace WebsiteParser.Classes.JsonParser;

internal class JsonParseManager<TConfig>(IJsonParser<TConfig> jsonParser)
    where TConfig : class, IConfig
{
    public async Task<JsonParseResult<TConfig>> JsonParseResultAsync (string filePath)
    {
        try
        {
            TConfig? parsedJson = await jsonParser.ParseFileAsync(filePath);
            if (parsedJson is null)
            {
                return new JsonParseError<TConfig>($"Файл {filePath} пуст или не найден.");
            }
            return new JsonParseSuccess<TConfig>(parsedJson, "Данные были успешно парсированы");
        }
        catch (Exception ex)
        {
            return new JsonParseError<TConfig>($"Провал парсинга: {ex.Message}");
        }
    }
}
