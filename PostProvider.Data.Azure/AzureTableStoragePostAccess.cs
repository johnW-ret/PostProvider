using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Options;
using PostProvider.Data.Services;
using PostProvider.Models;

namespace PostProvider.Data.Azure;

public class AzureTableStoragePostAccess : IPostsTableAccess
{
    private readonly TableServiceClient tableServiceClient;
    private readonly TableClient tableClient;
    private readonly TablesOptions tablesOptions;

    public AzureTableStoragePostAccess(
        TableServiceClient tableServiceClient,
        IOptions<TablesOptions> options)
    {
        this.tableServiceClient = tableServiceClient;
        tablesOptions = options.Value;

        tableClient = tableServiceClient.GetTableClient(tablesOptions.TableName);
    }

    public Task<(int, string)> AddRow(Row row)
    {
        return tableClient
            .AddEntityAsync((TableRow)row)
            .ContinueWith(t => new Func<Response, (int, string)>(
                r => (r.Status, r.ReasonPhrase))
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
}
