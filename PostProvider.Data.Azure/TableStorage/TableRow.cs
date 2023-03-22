using Azure;
using Azure.Data.Tables;
using PostProvider.Models;

namespace PostProvider.Data.Azure.TableStorage;

public class TableRow : ITableEntity
{
    public TableRow()
    {
        PartitionKey = CreatedOn.Year.ToString();
    }

    public TableRow(string key, string url, string name, DateTimeOffset createdOn)
    {
        Key = key;
        Url = url;
        Name = name;
        CreatedOn = createdOn;

        PartitionKey = GetPartitionAndRowKey(key).PartitionKey;
        ETag = new(Timestamp.GetHashCode().ToString());
    }

    public string Key { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateTimeOffset CreatedOn { get; set; }

    public string PartitionKey { get; set; }
    public string RowKey { get => Key; set => Key = value; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }

    public static explicit operator TableRow(Row row)
    {
        return new(row.Key, row.Url, row.Name, row.CreatedOn);
    }

    public static implicit operator Row(TableRow tableRow)
    {
        return new()
        {
            Id = tableRow.Key,
            Url = tableRow.Url,
            Name = tableRow.Name,
            CreatedOn = tableRow.CreatedOn
        };
    }

    public static TableRowKeys GetPartitionAndRowKey(string key)
        => new(key.FirstOrDefault().ToString(), key);
}

public record struct TableRowKeys(string PartitionKey, string RowKey);