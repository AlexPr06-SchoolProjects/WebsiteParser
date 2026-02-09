using WebsiteParser.Classes.AsyncLogger;
using WebsiteParser.Interfaces;
using WebsiteParser.Records;

namespace WebsiteParser.Classes.WebParser;

internal class WebParserClass(
    IHttpClientFactory httpClientFactory, 
    AsyncLoggerClass asyncLogger
    )
{
    public async Task<IWebsiteParseResult> FetchDataAsync(string url, CancellationToken token)
    {
        using HttpClient client = httpClientFactory.CreateClient("WebsiteParserClient");
        int maxRetries = 3;
        for (int i = 1; i <= maxRetries; ++i)
        {
            try
            {
                string html = await client.GetStringAsync(url, token);
                // LOGGIN logic
                await asyncLogger.LogAsync($"[green]Вебсайт: [seagreen1]{url}[/]      успешно пропарсирован.[/]");
                // LOGGIN logic
                return new WebsiteParseSuccess(html);
            }
            catch (HttpRequestException ex)
            {
                // LOGGIN logic
                await asyncLogger.LogAsync($"[red]HTTP Error: {ex.Message}.[/]");
                // LOGGIN logic
                return new WebsiteParseError($"HTTP Error: {ex.Message}.", ex.Message);
            }
            catch when (i < maxRetries)
            {
                await Task.Delay(1000 * (i + 1), token);
            }
            catch (Exception ex) 
            {
                // LOGGIN logic
                await asyncLogger.LogAsync($"[red]ERROR: {ex.Message}.[/]");
                // LOGGIN logic
                return new WebsiteParseError($"ERROR: {ex.Message}.", $"ERROR: {ex.Message}.");
            }
        }

        // LOGGIN logic
        await asyncLogger.LogAsync("[red]Failed after retries.[/]");
        // LOGGIN logic
        return new WebsiteParseError("Unknown", "Failed after retries.");
    }

    public async Task<IDictionary<string, IWebsiteParseResult>> FetchMultipleDataAsync(IEnumerable<string> urls, CancellationToken token)
    {
        var tasks = new Dictionary<string, Task<IWebsiteParseResult>>();

        foreach (string url in urls)
            tasks.Add(url, FetchDataAsync(url, token));

        await Task.WhenAll(tasks.Values);
        return tasks.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Result
            );
    }
}
