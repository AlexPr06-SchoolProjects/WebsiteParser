using WebsiteParser.Interfaces;

namespace WebsiteParser.Records;

internal record WebsiteParseSuccess(string Data, string Message = "OK") : IWebsiteParseResult;
internal record WebsiteParseError(string Data, string Message) : IWebsiteParseResult;
internal record WebsiteParseWarning(string Data, string Message) : IWebsiteParseResult;
