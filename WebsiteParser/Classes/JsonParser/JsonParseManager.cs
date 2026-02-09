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
                await asyncLogger.LogAsync($"[red]Файл [darkorange]{filePath}[/] пуст или не найден.[/]");
                // LOGGIN logic
                return new JsonParseError<TConfig>($"[red]Файл [darkorange]{filePath}[/] пуст или не найден.[/]");
            }
            // LOGGIN logic
            await asyncLogger.LogAsync($"[green]Данные файла [seagreen1]{filePath}[/] были успешно получены.[/]");
            // LOGGIN logic
            return new JsonParseSuccess<TConfig>(parsedJson, $"[green]Данные файла [seagreen1]{filePath}[/] были успешно получены.[/]");
        }
        catch (Exception ex)
        {
            // LOGGIN logic
            await asyncLogger.LogAsync($"[red]Провал парсинга: {ex.Message}.[/]");
            // LOGGIN logic
            return new JsonParseError<TConfig>($"[red]Провал парсинга: {ex.Message}.[/]");
        }
    }
}
