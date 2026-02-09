using Spectre.Console;
using System.Threading.Channels;
using WebsiteParser.Constants;

namespace WebsiteParser.Classes.AsyncLogger;

internal class AsyncLoggerClass
{
    private readonly Channel<string> _channel;
    private readonly Task _workerTask;
    private readonly CancellationTokenSource _cts = new();
    private readonly string _logsFilePath;
    public AsyncLoggerClass()
    {
        _channel = Channel.CreateBounded<string>(75);

        DirectoryPaths.CreateDirectoryIfNotExist(DirectoryPaths.LOGS_APP_FOLDER_PATH);
        _logsFilePath = Path.Combine(Path.GetFullPath(DirectoryPaths.LOGS_APP_FOLDER_PATH), FileNames.MAIN_LOG_FILE_NAME);
        
        _workerTask = Task.Run(() => ProcessLogsAsync(_cts.Token));
    }

    public async ValueTask LogAsync(string message)
    {
        await _channel.Writer.WriteAsync(message);
    }

    private async Task ProcessLogsAsync(CancellationToken token)
    {
        try
        {
            await foreach (var message in _channel.Reader.ReadAllAsync(token))
            {

                AnsiConsole.Write(CreateLogPanel($"[blue][[Logger Thread]][/]: {(message)}"));
                var plainText = message.RemoveMarkup();
                await File.AppendAllTextAsync(_logsFilePath, plainText + Environment.NewLine);
            }
        }
        catch(OperationCanceledException)
        {
            AnsiConsole.Write(CreateLogPanel($"[blue][[Logger Thread]][/]: [springgreen3]Все логи были сохраненны в файл [/]" +
                $"[seagreen1]{_logsFilePath}.[/]"));
            AnsiConsole.Write(CreateLogPanel($"[blue][[Logger Thread]][/]: {Markup.Escape($"Завершение работы...")}"));
        }
    }

    public async Task StopAsync()
    {
        _channel.Writer.Complete();
        _cts.Cancel();              
        await _workerTask;      
    }

    private Panel CreateLogPanel(string message)
    {
        var panel = new Panel(message)
        {
            Header = new PanelHeader("Log Entry"),
            Border = BoxBorder.Rounded,
            BorderStyle = new Style(Color.DarkGoldenrod)
        };
        return panel;
    }
}

