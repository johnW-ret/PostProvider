using PostProvider.Models;

namespace PostProvider.Data.Services;

public interface IPostClient
{
    public Task<Post?> GetPost(string name);
    public Task<TResponse<Post>> CreatePost(Post post);
}
