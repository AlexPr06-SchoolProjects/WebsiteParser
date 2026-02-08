using WebsiteParser.Records;

namespace WebsiteParser.Classes.WebParser;

internal class WebParserClass
{
    public async Task<ParseResult> FetchDataAsync(string url, CancellationToken token)
    {
        using HttpClient client = new HttpClient();
        ParseResult result;
        try
        {
            string response = await client.GetStringAsync(url, token);
            result = new Success(response);
        }
        catch(Exception ex)
        {
            result = new Error("404", $"ERROR: {ex.Message}");
        }

        return result;
    }

    public async Task<IDictionary<string, ParseResult>> FetchMultipleDataAsync(IEnumerable<string> urls, CancellationToken token)
    {
        var tasks = new Dictionary<string, Task<ParseResult>>();

        foreach (string url in urls)
            tasks.Add(url, FetchDataAsync(url, token));

        await Task.WhenAll(tasks.Values);
        return tasks.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Result
            );
    }
}
