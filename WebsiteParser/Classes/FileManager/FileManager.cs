using WebsiteParser.Records;

namespace WebsiteParser.Classes.FileManager;

internal class FileManager
{
    public FileGettingResult TryGetFileData(string fileDirectoryPath, string fileName)
    {
        FileGettingResult result;
        if (!File.Exists(fileDirectoryPath))
            result = new FileGettingError(fileName, $"ERROR: Direcotry does not exist");

        string fullRootPath = Path.GetFullPath(fileDirectoryPath);
        result = new FileGettingSuccess(fullRootPath);
        return result;
    }
}
