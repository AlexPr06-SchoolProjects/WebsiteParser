using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using WebsiteParser.Classes;
using WebsiteParser.Classes.AsyncLogger;
using WebsiteParser.Classes.FileManager;
using WebsiteParser.Classes.JsonParser;
using WebsiteParser.Classes.WebParser;
using WebsiteParser.Interfaces;

HostApplicationBuilder builder = new HostApplicationBuilder(args);

builder.Services.AddSingleton<App>();


#region Adding_Builder_Services
// ------------------------ Other Services --------------------------------------

builder.Services.AddSingleton<FileManagerClass>();

// WebParser services
builder.Services.AddSingleton<WebParserClass>();
builder.Services.AddSingleton<WebParserManager>();
builder.Services.AddTransient(typeof(IJsonParser<>), typeof(JsonParser<>));
builder.Services.AddTransient(typeof(JsonParseManager<>));
builder.Services.AddHttpClient("WebsiteParserClient", client =>
{
    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) Chrome/120.0.0.0 Safari/537.36");
    client.DefaultRequestHeaders.Add("Accept", "text/html");
    client.Timeout = TimeSpan.FromSeconds(10); 
});

// Async logger services
builder.Services.AddSingleton<AsyncLoggerClass>();


// ----------------------- OTPIONAL ------------------------------------

bool useHttpLogging = AnsiConsole.Confirm("Желаете использовать логирование, предоставленное встроенным логгером от IHttpClientFactory?");
if (!useHttpLogging)
{
    builder.Logging.AddFilter("System.Net.Http.HttpClient", LogLevel.Warning);
    builder.Logging.AddFilter("Microsoft.Extensions.Http", LogLevel.Error);
}

// ----------------------- OTPIONAL ------------------------------------

// --------------------------------------------------------------------------------
#endregion


using IHost host  = builder.Build();

App app = host.Services.GetRequiredService<App>(); 

await app.Run();  
