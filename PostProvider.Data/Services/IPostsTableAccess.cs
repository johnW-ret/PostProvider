using PostProvider.Models;
using System.Net;

namespace PostProvider.Data.Services;

public interface IPostsTableAccess
{
    public Task<Row?> GetRow(string key);
    public Task<List<Row>> GetRows(string filter, string? continuationToken);
    public Task<TResponse<Row>> AddRow(Row row);
    public Task<HttpStatusCode> DeleteRow(string key);
    public Task<TResponse<Row>> PutRow(Row row);
}