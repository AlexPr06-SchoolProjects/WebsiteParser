namespace WebsiteParser.Records;

internal abstract record FileGettingResult;

internal record FileGettingSuccess(string Data, string Message = "OK") : FileGettingResult;
internal record FileGettingError(string FilePath, string Message) : FileGettingResult;
internal record FileGettingWarning(string FilePath, string Message) : FileGettingResult;
