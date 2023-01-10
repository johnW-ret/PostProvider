using Microsoft.Extensions.DependencyInjection;
using PostProvider.Data.Services;

namespace PostProvider
{
    public interface IPostStorageOptionsBuilderInfrastructure
    {
        void AddImplementation<TImplementation>(string name)
            where TImplementation : IPostsTableAccess;
    }

    public class PostStorageOptionsBuilder : IPostStorageOptionsBuilderInfrastructure
    {
        private readonly IServiceCollection services;
        public PostStorageOptions Options { get; private set; } = new();

        public IServiceCollection Services => services;

        public PostStorageOptionsBuilder(IServiceCollection services)
        {
            this.services = services;
        }

        void IPostStorageOptionsBuilderInfrastructure.AddImplementation<TImplementation>(string name)
        {
            Options.Implementations.Add(name, typeof(TImplementation));
        }
    }
}