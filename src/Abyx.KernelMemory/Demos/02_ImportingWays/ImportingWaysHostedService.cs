using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.KernelMemory;

namespace Abyx.KernelMemory.Playground.Demos._02_ImportingWays;

public class ImportingWaysHostedService : IHostedService
{
    private ILogger<ImportingWaysHostedService> _logger;
    private readonly IKernelMemory _memory;

    public ImportingWaysHostedService(
        ILogger<ImportingWaysHostedService> logger,
        MemoryServerless memory)
    {
        _logger = logger;
        _memory = memory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await ImportWebUrl();
        await ImportWordDocument();
        await ImportPdfDocument();
        await ImportJsonDocument();
        await ImportCsvDocument();
        await ImportStreamDocument();
    }

    private async Task ImportWebUrl()
    {
        const string readmeUrl = "https://raw.githubusercontent.com/luismanez/global-ai-barcelona-2025/refs/heads/main/src/Abyx.KernelMemory.Playground/Data/about-us.md";
        Console.Write("Importing web url...");
        await _memory.ImportWebPageAsync(
            url: readmeUrl,
            documentId: "abyx-readme");
        Console.WriteLine("DONE");
    }

    private async Task ImportWordDocument()
    {
        Console.Write("Importing DOCX file...");
        await _memory.ImportDocumentAsync(
            filePath: "Data/employee-handbook.docx",
            documentId: "employee-handbook");
        Console.WriteLine("DONE");
    }

    private async Task ImportPdfDocument()
    {
        Console.Write("Importing PDF file...");
        await _memory.ImportDocumentAsync(
            filePath: "Data/branding-guidelines.pdf",
            documentId: "branding-guidelines");
        Console.WriteLine("DONE");
    }

    private async Task ImportJsonDocument()
    {
        Console.Write("Importing JSON file...");
        await _memory.ImportDocumentAsync(
            filePath: "Data/people-directory.json",
            documentId: "people-directory");
        Console.WriteLine("DONE");
    }

    private async Task ImportCsvDocument()
    {
        Console.Write("Importing CSV file...");
        await _memory.ImportDocumentAsync(
            filePath: "Data/project-task-list.csv",
            documentId: "project-task-list");
        Console.WriteLine("DONE");
    }

    private async Task ImportStreamDocument()
    {
        const string productInfo = @"NovaSense: A sleek wristband that continuously learns your personal routines
            through advanced machine learning algorithms, forecasting health fluctuations and offering tailored fitness insights in real time.
            Engineered to adapt to your daily patterns, NovaSense integrates seamlessly with other Abyx AI-driven gadgets,
            creating a holistic, hyper-personalized user experience.";

        var stream = new MemoryStream(Encoding.UTF8.GetBytes(productInfo));

        Console.Write("Importing raw STREAM...");
        await _memory.ImportDocumentAsync(
            new Document("novasense")
                .AddStream("novasense.txt", stream));
        Console.WriteLine("DONE");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogWarning("ImportingHostedService Stopped");
        return Task.CompletedTask;    }
}
