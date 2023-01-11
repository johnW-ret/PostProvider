namespace PostProvider;

public interface IPostStorageOptionsBuilderInfrastructure
{
    void AddImplementation<TService, TImplementation>(string name)
        where TImplementation : TService;
}
