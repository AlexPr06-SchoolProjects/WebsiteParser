namespace WebsiteParser.Constants;

internal static class FileAndDirectoriesPathsAndNames
{
    public const string WEBSITES_PATHS = "./Data/website-paths.json";
}

internal static class FileNames
{
    public const string MAIN_LOG_FILE_NAME = "app-logs.txt";
}

internal static class DirectoryPaths
{
    public const string SAVE_RESULT_FOLDER_PATH = "SavedResultsOfParsedWebsites";
    public const string LOGS_APP_FOLDER_PATH = "AppLogs";

    public static void CreateDirectoryIfNotExist(string dirPath)
    {
        string absolutePath = Path.GetFullPath(dirPath);

        if (!Directory.Exists(absolutePath))
            Directory.CreateDirectory(absolutePath);
    }
}
