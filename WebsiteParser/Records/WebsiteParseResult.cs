namespace WebsiteParser.Records;

internal abstract record WebsiteParseResult;

internal record WebsiteParseSuccess(string Data, string Message = "OK") : WebsiteParseResult;
internal record WebsiteParseError(string ErrorCode, string Message) : WebsiteParseResult;
internal record WebsiteParseWarning(int Code, string Message) : WebsiteParseResult;
