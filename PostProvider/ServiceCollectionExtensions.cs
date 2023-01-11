using Microsoft.Extensions.DependencyInjection;

namespace PostProvider;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPosts(
        this IServiceCollection services,
        Action<PostStorageOptionsBuilder> optionsAction)
    {
        var optionsBuilder = new PostStorageOptionsBuilder(services);
        optionsAction.Invoke(optionsBuilder);

        foreach (var implementationPair in optionsBuilder.Options.Implementations.Values)
        {
            foreach (var (service, implementation) in implementationPair)
            {
                services.AddScoped(service, implementation);
            }
        }

        return services;
    }
}