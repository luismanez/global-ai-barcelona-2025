using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.KernelMemory;
using Microsoft.KernelMemory.DocumentStorage.DevTools;
using Microsoft.KernelMemory.FileSystem.DevTools;

namespace Abyx.KernelMemory.Playground.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKernelMemory(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton(sp =>
        {
            var azureOpenAiTextConfig = new AzureOpenAIConfig();
            var azureOpenAiEmbeddingConfig = new AzureOpenAIConfig();
            var azureAiSearchConfig = new AzureAISearchConfig();

            configuration
                .BindSection("KernelMemory:Services:AzureOpenAIText", azureOpenAiTextConfig)
                .BindSection("KernelMemory:Services:AzureOpenAIEmbedding", azureOpenAiEmbeddingConfig)
                .BindSection("KernelMemory:Services:AzureAISearch", azureAiSearchConfig);

            var kmBuilder = new KernelMemoryBuilder()
                            .With(new KernelMemoryConfig { DefaultIndexName = "rag-km-demo" })
                            .WithAzureOpenAITextEmbeddingGeneration(
                                config: azureOpenAiEmbeddingConfig)
                            .WithAzureOpenAITextGeneration(
                                config: azureOpenAiTextConfig)
                            .WithAzureAISearchMemoryDb(azureAiSearchConfig)
                            .WithSimpleFileStorage(new SimpleFileStorageConfig
                            {
                                Directory = "/Users/luisman/temp/km-chunk-storage",
                                StorageType = FileSystemTypes.Disk
                            });

            kmBuilder.Services.AddLogging(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Error);
                builder.AddConsole();
            });

            var memory = kmBuilder.Build<MemoryServerless>(
                KernelMemoryBuilderBuildOptions.WithVolatileAndPersistentData);

            return memory;
        });

        return services;
    }
}
