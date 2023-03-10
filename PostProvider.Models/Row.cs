namespace PostProvider.Models;

public record class Row
{
    public Row() 
    {
        CreatedOn = DateTime.Now;
    }

    public Row(string url, string name) : this() 
    {
        Url = url;
        Name = name;
    }

    public string Url { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateTimeOffset CreatedOn { get; set; }
}