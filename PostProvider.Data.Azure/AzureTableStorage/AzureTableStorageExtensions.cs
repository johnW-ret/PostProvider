using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Azure;

namespace PostProvider.Data.Azure.AzureTableStorage;

public static class AzureTableStorageExtensions
{
    public class AzureTablePostStorageOptions
    {
        public IConfigurationSection? TablesSection { get; set; }
        public IConfigurationSection? BlobsSection { get; set; }
    }

    private static IServiceCollection AddAzureServiceClients(this IServiceCollection services, IConfigurationSection tableConfiguration, IConfigurationSection blobConfiguration)
    {
        services.AddAzureClients(clientBuilder =>
        {
            clientBuilder.AddTableServiceClient(tableConfiguration);

            clientBuilder.AddBlobServiceClient(blobConfiguration);
        });

        return services;
    }

    public static AzureTablePostStorageOptions SetTableClientConfigurationSection(
        this AzureTablePostStorageOptions options,
        IConfigurationSection configurationSection)
    {
        options.TablesSection = configurationSection;

        return options;
    }
    public static AzureTablePostStorageOptions SetBlobsClientConfigurationSection(
    this AzureTablePostStorageOptions options,
    IConfigurationSection configurationSection)
    {
        options.BlobsSection = configurationSection;

        return options;
    }

    public static PostStorageOptionsBuilder AddAzureStorage(this PostStorageOptionsBuilder options,
        Action<AzureTablePostStorageOptions> optionsAction)
    {
        // create Azure options
        var azureOptions = new AzureTablePostStorageOptions();

        // set options from fluent API
        optionsAction.Invoke(azureOptions);

        ArgumentNullException.ThrowIfNull(azureOptions.TablesSection);
        ArgumentNullException.ThrowIfNull(azureOptions.BlobsSection);

        options.Services.AddAzureServiceClients(azureOptions.TablesSection, azureOptions.BlobsSection);

        ((IPostStorageOptionsBuilderInfrastructure)options).AddImplementation<AzureTableStoragePostAccess>("Azure");

        return options;
    }
}