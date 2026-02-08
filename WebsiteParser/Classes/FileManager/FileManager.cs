using WebsiteParser.Classes.JsonParser;
using WebsiteParser.Interfaces;
using WebsiteParser.Records;
using WebsiteParser.Enums;

namespace WebsiteParser.Classes.FileManager;

internal class FileManagerClass(
        JsonParseManager<Config> jsonParseManager
    )
{
    public async Task<FileGettingResult> GetJsonPathsAsync(string filePath)
    {
        string fullFilePath = Path.GetFullPath(filePath);

        if (!File.Exists(fullFilePath))
            return new JsonFileGettingError(fullFilePath, $"ERROR: Директории не существует");


        JsonParseResult<Config> jsonResult = await jsonParseManager.JsonParseResultAsync(fullFilePath);

        string summary = jsonResult switch
        {
            JsonParseSuccess<Config> success => $"Готово! Сайтов в списке: {success.Data.Sites.Count}",
            JsonParseError<Config> error => $"Провал: {error.Message}",
            JsonParseWarning<Config> warning => $"Внимание: {warning.Message}. Но данные есть.",
            _ => "Неизвестный статус"
        };

        if (jsonResult is JsonParseSuccess<Config> successfullyParsed)
        {
            Config data = successfullyParsed.Data;
            List<string> sitePaths = new List<string>();
            foreach (string path in data.Sites)
                sitePaths.Add(path);
            return new JsonFileGettingSuccess(sitePaths, summary);
        }

        FileGettingResult result = jsonResult switch
        {
            JsonParseError<Config> error => new JsonFileGettingError(filePath, summary),
            JsonParseWarning<Config> warning => new JsonFileGettingWarning(filePath, summary),
            _ => new JsonFileGettingError("", $"Неожиданный тип результата парсинга файла: {jsonResult.GetType().Name}")
        };
        return result;
    }

    public async Task<List<IFileWrittenResult>> WriteMultipleFilesAsync(IDictionary<string, string> fileNamesAndText, string targetDirectory, FileExtentions fileExtention)
    {
        List<IFileWrittenResult> results = new List<IFileWrittenResult>();
        if (!Directory.Exists(targetDirectory))
            Directory.CreateDirectory(targetDirectory);

        foreach (KeyValuePair<string, string> kvp in fileNamesAndText)
        {
            try
            {
                string cleanFileName = Path.GetFileNameWithoutExtension(kvp.Key);
                string ext = fileExtention.ToString().ToLower();
                string fullPathToWrite = Path.Combine(targetDirectory, $"{cleanFileName}.{ext}");
                using FileStream fs = File.OpenWrite(fullPathToWrite);
                using StreamWriter sw = new StreamWriter(fs);
                await sw.WriteAsync(kvp.Value);
                results.Add(new FileWrittenSuccess($"{cleanFileName} был успешно записан."));
            }
            catch (Exception ex)
            {
                results.Add(new FileWrittenFailure($"Ошибка записи в файл: {ex.Message}"));
            }
        }
        return results;
    }
}
