using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Options;
using PostProvider.Data.Services;
using PostProvider.Models;
using System.Linq.Expressions;
using System.Net;

namespace PostProvider.Data.Azure.TableStorage;

public class AzureTableStoragePostTableAccess : IPostsTableAccess
{
    private readonly TableServiceClient tableServiceClient;
    private readonly TableClient tableClient;
    private readonly TablesOptions tablesOptions;

    public AzureTableStoragePostTableAccess(
        TableServiceClient tableServiceClient,
        IOptions<TablesOptions> options)
    {
        this.tableServiceClient = tableServiceClient;
        tablesOptions = options.Value;

        tableClient = tableServiceClient.GetTableClient(tablesOptions.TableName);
    }

    public Task<TResponse<Row>> AddRow(Row row)
    {
        return tableClient
            .AddEntityAsync((TableRow)row)
            .ContinueWith(t => new Func<Response, TResponse<Row>>(
                r => new(row, (HttpStatusCode)r.Status))
            (t.Result));
    }

    public async Task<HttpStatusCode> DeleteRow(string key)
    {
        var (partitionKey, rowKey) = TableRow.GetPartitionAndRowKey(key);
        var response = await tableClient.DeleteEntityAsync(partitionKey, rowKey);

        return (HttpStatusCode)response.Status;
    }

    public Task<Row?> GetRow(string key)
    {
        return Task.Run(() => tableClient
            .QueryAsync<TableRow>(ent => ent.Key == key)
            .ToBlockingEnumerable()
            .FirstOrDefault() switch
        {
            null => null,
            TableRow r => (Row)r
        });
    }

    public Task<List<Row>> GetRows(string filter, string? continuationToken = null)
    {
        return Task.Run(() => tableClient
            .QueryAsync<TableRow>(filter)
            .AsPages(continuationToken)
            .ToBlockingEnumerable()
            .FirstOrDefault()?
            .Values
            .Select(tableRow => (Row)tableRow)
            .ToList() ?? new List<Row>());
    }

    public Task<TResponse<Row>> PutRow(Row row)
        => Task.Run(() =>
        {
            if (tableClient
                .QueryAsync<TableRow>(ent => ent.Key == row.Key)
                .ToBlockingEnumerable()
                .FirstOrDefault() is not TableRow tableRow)
                return null;

            return tableClient
                .UpdateEntityAsync((TableRow)row, ETag.All)
                .ContinueWith(t => new Func<Response, TResponse<Row>>(
                    r => new(row, (HttpStatusCode)r.Status))
                (t.Result));
        });
}
