using System.Reflection.Metadata;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.KernelMemory;

namespace Abyx.KernelMemory.Playground.Demos._05_CustomImportHandler;

public class CustomImportHandlerHostedService : IHostedService
{
    private readonly ILogger<CustomImportHandlerHostedService> _logger;
    private readonly MemoryServerless _memory;

    public CustomImportHandlerHostedService(
        ILogger<CustomImportHandlerHostedService> logger,
        MemoryServerless memory)
    {
        _memory = memory;
        _logger = logger;
    }
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _memory.Orchestrator.AddHandler<CustomImportHandler>("my_custom_step");

        await _memory.ImportDocumentAsync(
            "Data/about-us.md",
            steps: [
                Constants.PipelineStepsExtract,
                Constants.PipelineStepsPartition,
                Constants.PipelineStepsGenEmbeddings,
                "my_custom_step",
                Constants.PipelineStepsSaveRecords
            ],
            cancellationToken: cancellationToken);

        Console.WriteLine("Import completed");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogWarning("CustomImportHandlerHostedService Stopped");
        return Task.CompletedTask;
    }
}
