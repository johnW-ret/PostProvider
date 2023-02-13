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

    public Task<Row?> GetRow(string name)
    {
        return Task.Run(() => tableClient
            .QueryAsync<TableRow>(ent => ent.Name == name)
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
}
