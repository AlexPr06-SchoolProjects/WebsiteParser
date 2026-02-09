using WebsiteParser.Classes.JsonParser;
using WebsiteParser.Interfaces;
using WebsiteParser.Records;
using WebsiteParser.Enums;
using WebsiteParser.Classes.AsyncLogger;
using WebsiteParser.Constants;

namespace WebsiteParser.Classes.FileManager;

internal class FileManagerClass(
        JsonParseManager<Config> jsonParseManager,
        AsyncLoggerClass asyncLogger
    )
{
    public async Task<FileGettingResult> GetJsonPathsAsync(string filePath)
    {
        string fullFilePath = Path.GetFullPath(filePath);

        if (!File.Exists(fullFilePath))
        {
            // LOGGIN logic
            await asyncLogger.LogAsync($"[red]ERROR: Директории не существует.[/]");
            // LOGGIN logic
            return new JsonFileGettingError(fullFilePath, $"[red]ERROR: Директории не существует.[/]");
        }

        JsonParseResult<Config> jsonResult = await jsonParseManager.JsonParseResultAsync(fullFilePath);

        string summary = jsonResult switch
        {
            JsonParseSuccess<Config> success => $"[green]Готово! Сайтов в списке: {success.Data.Sites.Count}.[/]",
            JsonParseError<Config> error => $"[red]Провал: {error.Message}.[/]",
            JsonParseWarning<Config> warning => $"[orange1]Внимание: {warning.Message}. Но данные есть.[/]",
            _ => "[grey]Неизвестный статус.[/]"
        };

        // LOGGIN logic
        await asyncLogger.LogAsync($"{summary}");
        // LOGGIN logic

        if (jsonResult is JsonParseSuccess<Config> successfullyParsed)
        {
            Config data = successfullyParsed.Data;
            List<string> sitePaths = new List<string>();
            foreach (string path in data.Sites)
                sitePaths.Add(path);
            return new JsonFileGettingSuccess(sitePaths, summary);
        }

        FileGettingResult result;
        switch (jsonResult)
        {
            case JsonParseError<Config> error:
                result = new JsonFileGettingError(filePath, summary);
                break;
            case JsonParseWarning<Config> warning:
                result = new JsonFileGettingWarning(filePath, summary);
                break;
            default:
                // LOGGIN logic
                await asyncLogger.LogAsync($"[red]Неожиданный тип результата парсинга файла: {jsonResult.GetType().Name}.[/]");
                // LOGGIN logic
                result = new JsonFileGettingError("", $"[red]Неожиданный тип результата парсинга файла: {jsonResult.GetType().Name}.[/]");
                break;
        }

        return result;
    }

    public async Task<List<IFileWrittenResult>> WriteMultipleFilesAsync(IDictionary<string, string> fileNamesAndText, string targetDirectory, FileExtentions fileExtention)
    {
        List<IFileWrittenResult> results = new List<IFileWrittenResult>();
        DirectoryPaths.CreateDirectoryIfNotExist(targetDirectory);

        foreach (KeyValuePair<string, string> kvp in fileNamesAndText)
        {
            try
            {
                string cleanFileName = Path.GetFileNameWithoutExtension(kvp.Key);
                string ext = fileExtention.ToString().ToLower();
                string fullPathToWrite = Path.Combine(targetDirectory, $"{cleanFileName}.{ext}");
                using FileStream fs = File.Create(fullPathToWrite);
                using StreamWriter sw = new StreamWriter(fs);
                await sw.WriteAsync(kvp.Value);
                results.Add(new FileWrittenSuccess($"[seagreen1]{cleanFileName}[/] [green]был успешно записан.[/]"));
                await asyncLogger.LogAsync($"[seagreen1]{cleanFileName}[/] [green]был успешно записан.[/]");
            }
            catch (Exception ex)
            {
                results.Add(new FileWrittenFailure($"[red]Ошибка записи в файл: {ex.Message}.[/]"));
                await asyncLogger.LogAsync($"[red]Ошибка записи в файл: {ex.Message}.[/]");
            }
        }
        return results;
    }
}
