using PostProvider.Models;

namespace PostProvider.Data.Services;

public interface IPostsTableAccess
{
    public Task<Row?> GetRow(string key);
    public Task<(int, string)> AddRow(Row row);
}