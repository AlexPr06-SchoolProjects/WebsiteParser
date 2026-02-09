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
            await asyncLogger.LogAsync($"ERROR: Директории не существует");
            // LOGGIN logic
            return new JsonFileGettingError(fullFilePath, $"ERROR: Директории не существует");
        }

        JsonParseResult<Config> jsonResult = await jsonParseManager.JsonParseResultAsync(fullFilePath);

        string summary = jsonResult switch
        {
            JsonParseSuccess<Config> success => $"Готово! Сайтов в списке: {success.Data.Sites.Count}",
            JsonParseError<Config> error => $"Провал: {error.Message}",
            JsonParseWarning<Config> warning => $"Внимание: {warning.Message}. Но данные есть.",
            _ => "Неизвестный статус"
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
                await asyncLogger.LogAsync($"Неожиданный тип результата парсинга файла: {jsonResult.GetType().Name}");
                // LOGGIN logic
                result = new JsonFileGettingError("", $"Неожиданный тип результата парсинга файла: {jsonResult.GetType().Name}");
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
                results.Add(new FileWrittenSuccess($"{cleanFileName} был успешно записан."));
                await asyncLogger.LogAsync($"{cleanFileName} был успешно записан.");
            }
            catch (Exception ex)
            {
                results.Add(new FileWrittenFailure($"Ошибка записи в файл: {ex.Message}"));
                await asyncLogger.LogAsync($"Ошибка записи в файл: {ex.Message}");
            }
        }
        return results;
    }
}
