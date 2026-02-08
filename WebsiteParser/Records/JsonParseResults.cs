using WebsiteParser.Interfaces;

namespace WebsiteParser.Records;

internal abstract record JsonParseResult<T>(string Message);

internal record JsonParseSuccess<T>(T Data, string Message = "OK")
    : JsonParseResult<T>(Message);

internal record JsonParseError<T>(string Message)
    : JsonParseResult<T>(Message);

internal record JsonParseWarning<T>(string Message)
    : JsonParseResult<T>(Message);
