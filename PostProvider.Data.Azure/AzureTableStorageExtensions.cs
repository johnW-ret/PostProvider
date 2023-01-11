using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Azure;
using PostProvider.Data.Services;
using PostProvider.Data.Azure.TableStorage;
using PostProvider.Data.Azure.BlobStorage;

namespace PostProvider.Data.Azure.AzureTableStorage;

public static class AzureStorageExtensions
{
    private static IServiceCollection AddAzureServiceClients(this IServiceCollection services, IConfigurationSection tableConfiguration, IConfigurationSection blobConfiguration)
    {
        services.AddAzureClients(clientBuilder =>
        {
            clientBuilder.AddTableServiceClient(tableConfiguration);

            clientBuilder.AddBlobServiceClient(blobConfiguration);
        });

        return services;
    }

    public static PostStorageOptionsBuilder AddAzureStorage(
        this PostStorageOptionsBuilder options,
        IConfigurationSection configurationSection)
    {
        var (tablesSection, blobSection) = (
            configurationSection.GetSection("Tables"),
            configurationSection.GetSection("Blobs"));

        options.Services.Configure<TablesOptions>(tablesSection);
        options.Services.Configure<BlobOptions>(blobSection);

        ArgumentNullException.ThrowIfNull(tablesSection, nameof(tablesSection));
        ArgumentNullException.ThrowIfNull(blobSection, nameof(blobSection));

        options.Services.AddAzureServiceClients(tablesSection, blobSection);

        var optionsInfra = (IPostStorageOptionsBuilderInfrastructure)options;

        optionsInfra.AddImplementation<IPostsTableAccess, AzureTableStoragePostTableAccess>("Azure");
        optionsInfra.AddImplementation<IPostClient, AzureBlobStoragePostClient>("Azure");

        return options;
    }
}