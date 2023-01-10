using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

using PostProvider.Data.Services;

namespace PostProvider;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPosts(
        this IServiceCollection services,
        IConfigurationSection configurationSection,
        Action<PostStorageOptionsBuilder> optionsAction)
    {
        services.Configure<TablesOptions>(configurationSection);

        var optionsBuilder = new PostStorageOptionsBuilder(services);
        optionsAction.Invoke(optionsBuilder);

        foreach (var implementation in optionsBuilder.Options.Implementations.Values)
        {
            services.AddScoped(typeof(IPostsTableAccess), implementation);
        }

        return services;
    }
}