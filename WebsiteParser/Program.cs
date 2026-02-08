using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebsiteParser.Classes;
using WebsiteParser.Classes.WebParser;

HostApplicationBuilder builder = new HostApplicationBuilder();

builder.Services.AddSingleton<App>();

// ------------------------ Other Services --------------------------------------

builder.Services.AddSingleton<WebParserClass>();

// ------------------------ Other Services --------------------------------------

using IHost host  = builder.Build();

App app = host.Services.GetRequiredService<App>();  

app.Run();  
