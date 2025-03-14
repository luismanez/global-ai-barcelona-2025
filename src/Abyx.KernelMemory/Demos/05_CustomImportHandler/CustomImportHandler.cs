using System;
using Microsoft.Extensions.Logging;
using Microsoft.KernelMemory.Diagnostics;
using Microsoft.KernelMemory.Pipeline;

namespace Abyx.KernelMemory.Playground.Demos._05_CustomImportHandler;

public class CustomImportHandler : IPipelineStepHandler
{
    private readonly IPipelineOrchestrator _orchestrator;
    private readonly ILogger<CustomImportHandler> _log;
    public string StepName { get; }

    public CustomImportHandler(string stepName,
        IPipelineOrchestrator orchestrator,
        ILoggerFactory? loggerFactory = null)
    {
        this.StepName = stepName;
        this._orchestrator = orchestrator;
        this._log = (loggerFactory ?? DefaultLogger.Factory).CreateLogger<CustomImportHandler>();
    }

    public async Task<(ReturnType returnType, DataPipeline updatedPipeline)> InvokeAsync(DataPipeline pipeline, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"Index: {pipeline.Index}");
        Console.WriteLine($"Document Id: {pipeline.DocumentId}");
        Console.WriteLine($"Steps: {string.Join(", ", pipeline.Steps)}");
        Console.WriteLine($"Remaining Steps: {string.Join(", ", pipeline.RemainingSteps)}");

        var dataPipelineStatus = await _orchestrator.ReadPipelineSummaryAsync(pipeline.Index, pipeline.DocumentId, cancellationToken);
        Console.WriteLine($"Pipeline Status Last Update: {dataPipelineStatus!.LastUpdate}");

        return (ReturnType.Success, pipeline);
    }
}
