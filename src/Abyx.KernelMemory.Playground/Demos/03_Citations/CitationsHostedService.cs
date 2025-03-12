using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.KernelMemory;

namespace Abyx.KernelMemory.Playground.Demos._03_Citations;

public class CitationsHostedService : IHostedService
{
    private readonly ILogger<CitationsHostedService> _logger;
     private readonly IKernelMemory _memory;
    public CitationsHostedService(
        ILogger<CitationsHostedService> logger,
        MemoryServerless memory)
    {
        _logger = logger;
        _memory = memory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var question = "Who is Bob Miller's manager?";

        var answer = await _memory.AskAsync(question, cancellationToken: cancellationToken);

        Console.WriteLine($"Q: {question}\nA: {answer.Result}");
        Console.WriteLine();

        foreach (var citation in answer.RelevantSources)
        {
            Console.WriteLine(citation.SourceName.ToUpper());
            foreach (var snippet in citation.Partitions)
            {
                Console.WriteLine($">>> {snippet.Relevance} | {snippet.LastUpdate} | {snippet.Text[..50]}");
            }
            Console.WriteLine();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogWarning("CitationsHostedService Stopped");
        return Task.CompletedTask;
    }
}
