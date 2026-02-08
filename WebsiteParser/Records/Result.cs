namespace WebsiteParser.Records;

internal abstract record ParseResult;

internal record Success(string Data, string Message = "OK") : ParseResult;
internal record Error(string ErrorCode, string Message) : ParseResult;
internal record Warning(int Code, string Message) : ParseResult;
