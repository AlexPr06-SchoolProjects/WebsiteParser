using WebsiteParser.Interfaces;

namespace WebsiteParser.Records;

internal record WebParserManagerSuccess(string Message) : IWebParserManagerResult;
internal record WebParserManagerFailure(string Message) : IWebParserManagerResult;
