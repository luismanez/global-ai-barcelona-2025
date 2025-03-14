
using Abyx.KernelMemory.Playground.Demos._01_HelloKernelMemory;
using Abyx.KernelMemory.Playground.Demos._02_ImportingWays;
using Abyx.KernelMemory.Playground.Demos._03_Citations;
using Abyx.KernelMemory.Playground.Demos._04_SearchFilters;
using Abyx.KernelMemory.Playground.Demos._05_CustomImportHandler;
using Abyx.KernelMemory.Playground.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureHostConfiguration(configHost =>
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        configHost.SetBasePath(currentDirectory);
        configHost.AddJsonFile("appsettings.json", optional: false);
        configHost.AddCommandLine(args);
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddKernelMemory(hostContext.Configuration);

        var demoToRun = args.Length > 0 ? args[0] : "1";
        switch (demoToRun)
        {
            case "1":
                services.AddHostedService<HelloKernelMemoryHostedService>();
                break;
            case "2":
                services.AddHostedService<ImportingWaysHostedService>();
                break;
            case "3":
                services.AddHostedService<CitationsHostedService>();
                break;
            case "4":
                services.AddHostedService<SearchFiltersHostedService>();
                break;
            case "5":
                services.AddHostedService<CustomImportHandlerHostedService>();
                break;
            default:
                throw new ArgumentException($"Unknown demo: {demoToRun}");
        }
    }).Build();

var logger = host.Services.GetRequiredService<ILogger<IHost>>();
try
{
    host.Run();
}
catch (OptionsValidationException ex)
{
    foreach (var failure in ex.Failures)
    {
        logger!.LogError(failure);
    }
}