using System;
using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.KernelMemory;

namespace Abyx.KernelMemory.Playground.Demos._04_SearchFilters;

public class SearchFiltersHostedService : IHostedService
{
    private readonly ILogger<SearchFiltersHostedService> _logger;
    private readonly MemoryServerless _memory;

    public SearchFiltersHostedService(
        ILogger<SearchFiltersHostedService> logger,
        MemoryServerless memory)
    {
        _memory = memory;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        const string productInfo = @"HoloSight: A cutting-edge pair of augmented-reality glasses powered by deep-learning vision algorithms.
            HoloSight not only overlays real-time data onto your field of view
            but also anticipates your needs—whether it’s identifying objects in your environment
            or translating foreign texts—making the world around you smarter, safer, and infinitely more interactive.";

        var stream = new MemoryStream(Encoding.UTF8.GetBytes(productInfo));

        Console.Write("Importing raw STREAM...");
        await _memory.ImportDocumentAsync(
            new Document("holosight")
                .AddStream("holosight.txt", stream)
                .AddTag("marketplace", "NAM")
                .AddTag("marketplace", "EUR")
                .AddTag("category", "Product")
                .AddTag("category", "Gadget"));
        Console.WriteLine("DONE");

        Console.WriteLine("Waiting for memory ingestion to complete...");
        await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken);

        var question = "What do you know about HoloSight product?";
        var answer = new MemoryAnswer();

        // Do know the answer: (NAM and Product) or (Asia and Gadget)
        answer = await _memory.AskAsync(
            question,
            filters: new List<MemoryFilter>
            {
            MemoryFilters.ByTag("marketplace", "NAM")
                            // ... AND ...
                            .ByTag("category", "Product"),
            // ... OR ...
            MemoryFilters.ByTag("marketplace", "Asia")
                            // ... AND ...
                            .ByTag("category", "Gadget"),
            });

        Console.WriteLine($"\nQ: {question}\nA: {answer.Result}");

        // Do NOT know the answer: (NAM and Service) or (Asia and Gadget)
        answer = await _memory.AskAsync(
            question,
            filters: new List<MemoryFilter>
            {
            MemoryFilters.ByTag("marketplace", "NAM")
                            // ... AND ...
                            .ByTag("category", "Service"),
            // ... OR ...
            MemoryFilters.ByTag("marketplace", "Asia")
                            // ... AND ...
                            .ByTag("category", "Gadget"),
            });

        Console.WriteLine($"\nQ: {question}\nA: {answer.Result}");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogWarning("SearchFiltersHostedService Stopped");
        return Task.CompletedTask;    }
}
