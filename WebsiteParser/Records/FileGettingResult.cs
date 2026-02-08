using WebsiteParser.Interfaces;

namespace WebsiteParser.Records;

internal abstract record FileGettingResult(string Message) : IFileOperationResult;

internal record JsonFileGettingSuccess(List<string> paths, string Message = "OK") : FileGettingResult(Message);
internal record JsonFileGettingError(string FilePath, string Message) : FileGettingResult(Message);
internal record JsonFileGettingWarning(string FilePath, string Message) : FileGettingResult(Message);
