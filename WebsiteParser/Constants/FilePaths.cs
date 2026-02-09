namespace WebsiteParser.Constants;

internal static class FilePaths
{
    public const string WEBSITES_PATHS = "../../../Data/website-paths.json";
}

internal static class DirectoryPaths
{
    public const string SAVE_RESULT_FOLDER_PATH = "../../../SavedResultsOfParsedWebsites/";

    public static void CreateDirectoryIfNotExist(string dirPath)
    {
        string absolutePath = Path.GetFullPath(dirPath);

        if (!Directory.Exists(absolutePath))
            Directory.CreateDirectory(absolutePath);
    }
}
