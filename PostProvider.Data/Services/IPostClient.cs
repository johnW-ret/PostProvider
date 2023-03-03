using PostProvider.Models;

namespace PostProvider.Data.Services;

public interface IPostClient
{
    public Task<Post?> GetPost(string name);
    public Task<TResponse<Post>> CreatePost(PostInputs postInputs);
    public Task<TResponse<Post>> PutPost(PostInputs postInputs);
    public Task<bool> DeletePost(string key);
}
