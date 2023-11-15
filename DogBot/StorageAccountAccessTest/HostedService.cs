using Azure;
using Azure.Core.Diagnostics;
using Azure.Identity;
using Azure.Storage;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics.Tracing;

namespace StorageAccountAccessTest;

public sealed class HostedService : IHostedService
{
    private readonly ILogger _logger;
    private readonly AppSettings _settings;

    public HostedService(
        ILogger<HostedService> logger,
        IHostApplicationLifetime appLifetime,
        IOptions<AppSettings> settings)
    {
        _settings = settings.Value;
        _logger = logger;

        appLifetime.ApplicationStarted.Register(OnStarted);
        appLifetime.ApplicationStopping.Register(OnStopping);
        appLifetime.ApplicationStopped.Register(OnStopped);
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        //Console.WriteLine("Using account keys to authenticate to storage account.");
        //var storageSharedKeyCredential = new StorageSharedKeyCredential(_settings.AccountKeyMethod.AccountName, _settings.AccountKeyMethod.AccountKey);
        //var blobContainerClient = new BlobContainerClient(new Uri(_settings.BlobContainerUri), storageSharedKeyCredential);

        //Console.WriteLine("Using a connection string to authenticate to storage account.");
        //var blobContainerClient = new BlobContainerClient(_settings.ConnectionStringMethod.ConnectionString, _settings.ConnectionStringMethod.ContainerName);

        Console.WriteLine("Using managed identity to authenticate to storage account.");
        var defaultAzureCredentials = new DefaultAzureCredential();
        var blobContainerClient = new BlobContainerClient(new Uri(_settings.BlobContainerUri), defaultAzureCredentials);

        //Console.WriteLine("Using a SAS token to authenticate to storage account.");
        //var sasCredential = new AzureSasCredential(_settings.SasTokenMethod.SasToken);
        //var blobContainerClient = new BlobContainerClient(new Uri(_settings.BlobContainerUri), sasCredential);

        var blobList = blobContainerClient.GetBlobsAsync();

        Console.WriteLine("Printing blob names in container");

        await foreach (var blob in blobList)
        {
            Console.WriteLine(blob.Name);
        }

        await Task.Delay(0, cancellationToken);

        Console.ReadKey();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("4. StopAsync has been called.");

        return Task.CompletedTask;
    }

    private void OnStarted()
    {
        _logger.LogInformation("2. OnStarted has been called.");
        Console.WriteLine($"Test value (3): {_settings.BlobContainerUri}");

    }

    private void OnStopping()
    {
        _logger.LogInformation("3. OnStopping has been called.");
    }

    private void OnStopped()
    {
        _logger.LogInformation("5. OnStopped has been called.");
    }
}
