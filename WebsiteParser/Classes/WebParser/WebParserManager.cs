using WebsiteParser.Classes.FileManager;
using WebsiteParser.Interfaces;
using WebsiteParser.Records;
using WebsiteParser.Enums;
using WebsiteParser.Classes.AsyncLogger;
using WebsiteParser.Classes.NetworkHelper;

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
                JsonFileGettingSuccess s => $"[green]Успех: {s.Message}[/]",
                JsonFileGettingError e => $"[red]{e.FilePath}: {e.Message}[/]",
                JsonFileGettingWarning w => $"[orange1]{w.FilePath}: {w.Message}[/]",
                _ => "[grey]Неизвестный статус[/]"
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
        if (!await NetworkHelperClass.HasConnection())
        {
            await asyncLogger.LogAsync("[red]Ошибка: Интернет-соединение отсутствует. Проверьте кабель или Wi-Fi.[/]");
            return new WebParserManagerFailure("Ошибка: Интернет-соединение отсутствует. Проверьте кабель или Wi-Fi.");
        }

        IDictionary<string, IWebsiteParseResult>? parsedSites = await this.GetWebsitesHTMLAsync(jsonFileWithPaths, directoryPathToSaveHTML, token);
        if (parsedSites is null)
        {
            // LOGGIN logic
            await asyncLogger.LogAsync($"[red]Файл с путями сайтов не был найден.[/]");
            // LOGGIN logic
            return new WebParserManagerFailure($"Файл с путями сайтов не был найден.");
        }

        Dictionary<string, string> filesToHTML = new Dictionary<string, string>(parsedSites.Count);
        foreach (var (key, value) in parsedSites)
            filesToHTML.Add(key, value.Data);
        List<IFileWrittenResult> fileWrittenResults = await fileManagerClass.WriteMultipleFilesAsync(filesToHTML, directoryPathToSaveHTML, FileExtentions.HTML);
        // LOGGIN logic
        await asyncLogger.LogAsync($"[green]Все сайты из файла {jsonFileWithPaths} были пропарсированны и положены в директорию {directoryPathToSaveHTML}.[/]");
        // LOGGIN logic   
        return new WebParserManagerSuccess($"[springgreen3]Все сайты из файла {jsonFileWithPaths} были пропарсированны и положены в директорию {directoryPathToSaveHTML}.[/]");
    }
}
