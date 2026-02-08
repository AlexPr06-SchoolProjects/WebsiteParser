using WebsiteParser.Constants;
using WebsiteParser.Classes.WebParser;
using WebsiteParser.Interfaces;

namespace WebsiteParser.Classes;

internal class App(WebParserManager webParserManager)
{
    async public Task Run()
    {
        CancellationTokenSource cts = new CancellationTokenSource();

        IWebParserManagerResult webParserManagerResult = await webParserManager.ParseWebsites(FilePaths.WEBSITES_PATHS, DirectoryPaths.SAVE_RESULT_FOLDER_PATH, cts.Token);
        Console.WriteLine(webParserManagerResult.Message);
    }
}
