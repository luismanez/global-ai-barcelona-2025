using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.KernelMemory;

namespace Abyx.KernelMemory.Playground.Demos._01_HelloKernelMemory;

public class HelloKernelMemoryHostedService : IHostedService
{
    private readonly ILogger<HelloKernelMemoryHostedService> _logger;
    private readonly IKernelMemory _memory;

    public HelloKernelMemoryHostedService(
        ILogger<HelloKernelMemoryHostedService> logger,
        MemoryServerless memory)
    {
        _memory = memory;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var documentId = await _memory.ImportTextAsync(@"
            Founded in 2021 by visionary engineer Emma Wright,
            Abyx pioneers the next generation of AI-driven gadgets
            that seamlessly merge futuristic technology with everyday convenience,
            proving that the future is closer than you think.", cancellationToken: cancellationToken);

        // not what you expect: it just checks if the document is in he pipeline json file, but not in the Vector DB
        // while (!await _memory.IsDocumentReadyAsync(documentId, cancellationToken: cancellationToken))

        Console.WriteLine("Waiting for memory ingestion to complete...");
        await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken);

        var query = "Who and when was created Abyx?";

        var answer = await _memory.AskAsync(
            query,
            cancellationToken: cancellationToken);

        Console.WriteLine($"Question: {query}\n\nAnswer: {answer.Result}");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogWarning("HelloSemanticWorldHostedService Stopped");
        return Task.CompletedTask;
    }
}
