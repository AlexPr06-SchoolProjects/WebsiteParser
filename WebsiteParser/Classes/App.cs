using WebsiteParser.Constants;
using WebsiteParser.Classes.WebParser;
using WebsiteParser.Interfaces;
using WebsiteParser.Classes.AsyncLogger;

namespace WebsiteParser.Classes;

internal class App(
    WebParserManager webParserManager,
    AsyncLoggerClass asyncLogger
    )
{
    async public Task Run()
    {
        CancellationTokenSource cts = new CancellationTokenSource();

        IWebParserManagerResult webParserManagerResult = await webParserManager.ParseWebsites(FilePaths.WEBSITES_PATHS, DirectoryPaths.SAVE_RESULT_FOLDER_PATH, cts.Token);
        await asyncLogger.LogAsync(webParserManagerResult.Message);
        await asyncLogger.StopAsync();
    }
}
