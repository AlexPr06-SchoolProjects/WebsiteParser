using WebsiteParser.Constants;
using WebsiteParser.Classes.WebParser;

namespace WebsiteParser.Classes;

internal class App(WebParserManager webParserManager)
{
    async public Task Run()
    {
        CancellationTokenSource cts = new CancellationTokenSource();

        await webParserManager.ParseWebsites(FilePaths.WEBSITES_PATHS, DirectoryPaths.SAVE_RESULT_FOLDER_PATH, cts.Token);
    }
}
