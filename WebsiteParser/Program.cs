using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebsiteParser.Classes;
using WebsiteParser.Classes.JsonParser;
using WebsiteParser.Classes.WebParser;
using WebsiteParser.Interfaces;
using WebsiteParser.Records;

HostApplicationBuilder builder = new HostApplicationBuilder();

builder.Services.AddSingleton<App>();

// ------------------------ Other Services --------------------------------------

builder.Services.AddSingleton<WebParserClass>();

// WebParser services

builder.Services.AddTransient(typeof(IJsonParser<>), typeof(JsonParser<>));
builder.Services.AddTransient(typeof(JsonParseManager<>));

// ------------------------ Other Services --------------------------------------

using IHost host  = builder.Build();

App app = host.Services.GetRequiredService<App>(); 

app.Run();  
