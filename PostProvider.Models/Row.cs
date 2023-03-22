namespace PostProvider.Models;

public record class Row
{
    public Row() 
    {
        CreatedOn = DateTime.Now;
    }

    public Row(string id, string url, string name) : this() 
    {
        Id = id;
        Url = url;
        Name = name;
    }

    public string Id { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Key => Id;
    public DateTimeOffset CreatedOn { get; set; }
}