using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebsiteParser.Classes;

HostApplicationBuilder builder = new HostApplicationBuilder();

builder.Services.AddSingleton<App>();

// ------------------------ Other Services --------------------------------------

// ------------------------ Other Services --------------------------------------

using IHost host  = builder.Build();

App app = host.Services.GetRequiredService<App>();  

app.Run();  
