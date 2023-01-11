using PostProvider.Models;

namespace PostProvider.Data.Services;

public interface IPostsTableAccess
{
    public Task<Row?> GetRow(string key);
    public Task<TResponse<Row>> AddRow(Row row);
}