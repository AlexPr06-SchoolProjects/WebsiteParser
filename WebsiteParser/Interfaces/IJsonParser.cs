namespace WebsiteParser.Interfaces;

internal interface IJsonParser<TConfig> where TConfig : IConfig
{
    public Task<TConfig?> ParseFileAsync(string filePath);
}