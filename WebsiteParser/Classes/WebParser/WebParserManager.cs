using WebsiteParser.Classes.FileManager;
using WebsiteParser.Interfaces;
using WebsiteParser.Records;
using WebsiteParser.Enums;
using WebsiteParser.Classes.AsyncLogger;

namespace WebsiteParser.Classes.WebParser;

internal class WebParserManager(
        WebParserClass webParserClass,
        FileManagerClass fileManagerClass,
        AsyncLoggerClass asyncLogger
    )
{
    private async Task<IDictionary<string, IWebsiteParseResult>?> GetWebsitesHTMLAsync(string jsonFileWithPaths, string directoryPathToSaveHTML, CancellationToken token)
    {
        List<string> paths = new List<string>();
        FileGettingResult fileGettingResult = await fileManagerClass.GetJsonPathsAsync(jsonFileWithPaths);


        if (fileGettingResult is not JsonFileGettingSuccess)
        {
            // LOGGIN logic
            string message = fileGettingResult switch
            {
                JsonFileGettingError e => $"{e.FilePath}: {e.Message}",
                JsonFileGettingWarning w => $"{w.FilePath}: {w.Message}",
                JsonFileGettingSuccess s => $"Успех: {s.Message}",
                _ => "Неизвестный статус"
            };
            await asyncLogger.LogAsync(message);
            // LOGGIN logic
            return null;
        }

        JsonFileGettingSuccess succeededResult = (JsonFileGettingSuccess)fileGettingResult;
        foreach (string path in succeededResult.paths)
            paths.Add(path);


        return await webParserClass.FetchMultipleDataAsync(paths, token);
    }

    public async Task<IWebParserManagerResult> ParseWebsites(string jsonFileWithPaths, string directoryPathToSaveHTML, CancellationToken token)
    {
        IDictionary<string, IWebsiteParseResult>? parsedSites = await this.GetWebsitesHTMLAsync(jsonFileWithPaths, directoryPathToSaveHTML, token);
        if (parsedSites is null)
        {
            // LOGGIN logic
            await asyncLogger.LogAsync($"Файл с путями сайтов не был найден.");
            // LOGGIN logic
            return new WebParserManagerFailure($"Файл с путями сайтов не был найден.");
        }

        Dictionary<string, string> filesToHTML = new Dictionary<string, string>(parsedSites.Count);
        foreach (var (key, value) in parsedSites)
            filesToHTML.Add(key, value.Data);
        List<IFileWrittenResult> fileWrittenResults = await fileManagerClass.WriteMultipleFilesAsync(filesToHTML, directoryPathToSaveHTML, FileExtentions.HTML);
        // LOGGIN logic
        await asyncLogger.LogAsync($"Все сайты из файла {jsonFileWithPaths} были пропарсированны и положены в директорию {directoryPathToSaveHTML}.");
        // LOGGIN logic   
        return new WebParserManagerSuccess($"Все сайты из файла {jsonFileWithPaths} были пропарсированны и положены в директорию {directoryPathToSaveHTML}.");
    }
}
