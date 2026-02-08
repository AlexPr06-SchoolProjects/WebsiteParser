using WebsiteParser.Interfaces;

namespace WebsiteParser.Records;
internal record FileWrittenSuccess(string Message) : IFileWrittenResult;
internal record FileWrittenFailure(string Message) : IFileWrittenResult;
