using System;
using System.IO;

public static class LogManager
{
    private static readonly string logDirectoryPath;

    static LogManager()
    {
        string baseDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        string programName = Path.GetFileNameWithoutExtension(AppDomain.CurrentDomain.FriendlyName);
        logDirectoryPath = Path.Combine(baseDirectoryPath, "VkLogs", programName);
    }

    public static void LogError(Exception ex)
    {
        string logFilePath = GetLogFilePath();

        using (StreamWriter writer = new StreamWriter(logFilePath, true))
        {
            writer.WriteLine($"[{DateTime.Now}] Error: {ex.Message}");
            writer.WriteLine($"StackTrace: {ex.StackTrace}");
            writer.WriteLine();
        }

        string screenshotFilePath = GetScreenshotFilePath();

        // делаем скриншот
        // ...

        // сохраняем скриншот по пути screenshotFilePath
        // ...
    }

    public static void LogMessage(string message, string fileName)
    {
        string basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "VkLogs");
        string programName = Path.GetFileNameWithoutExtension(AppDomain.CurrentDomain.FriendlyName);
        string folderPath = Path.Combine(basePath, programName, DateTime.Now.ToString("yyyy-MM-dd"));
        string filePath = Path.Combine(folderPath, fileName);

        Directory.CreateDirectory(folderPath);

        try
        {
            using (var writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine(message);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка записи в лог-файл: {ex.Message}");
        }
    }


    private static string GetLogFilePath()
    {
        string logDirectory = GetOrCreateDirectory(logDirectoryPath);
        string logFileName = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".txt";
        return Path.Combine(logDirectory, logFileName);
    }

    private static string GetScreenshotFilePath()
    {
        string logDirectory = GetOrCreateDirectory(logDirectoryPath);
        string screenshotFileName = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".jpg";
        return Path.Combine(logDirectory, screenshotFileName);
    }

    private static string GetOrCreateDirectory(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        return directoryPath;
    }
}

