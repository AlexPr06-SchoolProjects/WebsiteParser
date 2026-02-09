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

        DirectoryPaths.CreateDirectoryIfNotExist(DirectoryPaths.SAVE_RESULT_FOLDER_PATH);
        _logsFilePath = Path.Combine(Path.GetFullPath(DirectoryPaths.SAVE_RESULT_FOLDER_PATH), "logs.txt");
        
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
                Console.WriteLine($"[Logger Thread]: {message}");
                await File.AppendAllTextAsync(_logsFilePath, message + Environment.NewLine);
            }
        }
        catch(OperationCanceledException)
        {
            Console.WriteLine("[Logger Thread]: Завершение работы...");
        }
    }

    public async Task StopAsync()
    {
        _channel.Writer.Complete();
        _cts.Cancel();              
        await _workerTask;      
    }
}

