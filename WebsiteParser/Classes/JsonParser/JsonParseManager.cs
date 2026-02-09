using WebsiteParser.Interfaces;
using WebsiteParser.Records;
using WebsiteParser.Classes.AsyncLogger;

namespace WebsiteParser.Classes.JsonParser;

internal class JsonParseManager<TConfig>(
    IJsonParser<TConfig> jsonParser,
    AsyncLoggerClass asyncLogger
    )
    where TConfig : class, IConfig
{
    public async Task<JsonParseResult<TConfig>> JsonParseResultAsync (string filePath)
    {
        try
        {
            TConfig? parsedJson = await jsonParser.ParseFileAsync(filePath);
            if (parsedJson is null)
            {
                // LOGGIN logic
                await asyncLogger.LogAsync($"[red]Файл {filePath} пуст или не найден.[/]");
                // LOGGIN logic
                return new JsonParseError<TConfig>($"Файл {filePath} пуст или не найден.");
            }
            // LOGGIN logic
            await asyncLogger.LogAsync($"[green]Данные были успешно парсированы.[/]");
            // LOGGIN logic
            return new JsonParseSuccess<TConfig>(parsedJson, "Данные были успешно парсированы.");
        }
        catch (Exception ex)
        {
            // LOGGIN logic
            await asyncLogger.LogAsync($"[red]Провал парсинга: {ex.Message}.[/]");
            // LOGGIN logic
            return new JsonParseError<TConfig>($"Провал парсинга: {ex.Message}.");
        }
    }
}
