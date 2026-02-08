using WebsiteParser.Interfaces;

namespace WebsiteParser.Classes.JsonParser;

public class Config : IConfig
{
    public List<string> Sites { get; set; } = new List<string>();
}
