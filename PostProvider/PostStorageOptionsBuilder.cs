using Microsoft.Extensions.DependencyInjection;

namespace PostProvider;

public class PostStorageOptionsBuilder : IPostStorageOptionsBuilderInfrastructure
{
    private readonly IServiceCollection services;
    public PostStorageOptions Options { get; private set; } = new();

    public IServiceCollection Services => services;

    public PostStorageOptionsBuilder(IServiceCollection services)
    {
        this.services = services;
    }

    void IPostStorageOptionsBuilderInfrastructure.AddImplementation<TService, TImplementation>(string name)
    {
        if (!Options.Implementations.TryGetValue(name, out var typePairs))
            Options.Implementations.Add(name, typePairs = new());

        typePairs.Add((typeof(TService), typeof(TImplementation)));
    }
}