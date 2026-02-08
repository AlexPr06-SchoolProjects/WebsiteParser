using WebsiteParser.Classes.FileManager;
using WebsiteParser.Interfaces;
using WebsiteParser.Records;
using WebsiteParser.Constants;
using WebsiteParser.Enums;

namespace WebsiteParser.Classes.WebParser;

internal class WebParserManager(
        WebParserClass webParserClass,
        FileManagerClass fileManagerClass
    )
{
    private async Task<IDictionary<string, IWebsiteParseResult>> GetWebsitesHTMLAsync(string jsonFileWithPaths, string directoryPathToSaveHTML, CancellationToken token)
    {
        List<string> paths = new List<string>();
        FileGettingResult fileGettingResult = await fileManagerClass.GetJsonPathsAsync(jsonFileWithPaths);


        if (fileGettingResult is not JsonFileGettingSuccess)
        {

            // TODO : IMPLEMENT logging 

            //----------------------- To Change
            //string message = fileGettingResult switch
            //{
            //    JsonFileGettingError e => $"{e.FilePath}: {e.Message}",
            //    JsonFileGettingWarning w => $"{w.FilePath}: {w.Message}",
            //    JsonFileGettingSuccess s => $"Успех: {s.Message}",
            //    _ => "Неизвестный статус"
            //};
            ///----------------------- To Change
        }

        JsonFileGettingSuccess succeededResult = (JsonFileGettingSuccess)fileGettingResult;
        foreach (string path in succeededResult.paths)
            paths.Add(path);


        return await webParserClass.FetchMultipleDataAsync(paths, token);
    }

    public async Task ParseWebsites(string jsonFileWithPaths, string directoryPathToSaveHTML, CancellationToken token)
    {
        IDictionary<string, IWebsiteParseResult> parsedSites = await this.GetWebsitesHTMLAsync(jsonFileWithPaths, directoryPathToSaveHTML, token);
        // LOGGIN logic

        // LOGGIN logic

        Dictionary<string, string> filesToHTML = new Dictionary<string, string>(parsedSites.Count);
        foreach (var (key, value) in parsedSites)
            filesToHTML.Add(key, value.Data);
        List<IFileWrittenResult> fileWrittenResults = await fileManagerClass.WriteMultipleFilesAsync(filesToHTML, DirectoryPaths.SAVE_RESULT_FOLDER_PATH, FileExtentions.HTML);
        // LOGGIN logic

        // LOGGIN logic   
    }
}
