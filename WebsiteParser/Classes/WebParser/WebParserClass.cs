using WebsiteParser.Records;

namespace WebsiteParser.Classes.WebParser;

internal class WebParserClass
{
    public async Task<WebsiteParseResult> FetchDataAsync(string url, CancellationToken token)
    {
        using HttpClient client = new HttpClient();
        WebsiteParseResult result;
        try
        {
            string response = await client.GetStringAsync(url, token);
            result = new WebsiteParseSuccess(response);
        }
        catch(Exception ex)
        {
            result = new WebsiteParseError("404", $"ERROR: {ex.Message}");
        }

        return result;
    }

    public async Task<IDictionary<string, WebsiteParseResult>> FetchMultipleDataAsync(IEnumerable<string> urls, CancellationToken token)
    {
        var tasks = new Dictionary<string, Task<WebsiteParseResult>>();

        foreach (string url in urls)
            tasks.Add(url, FetchDataAsync(url, token));

        await Task.WhenAll(tasks.Values);
        return tasks.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Result
            );
    }
}
