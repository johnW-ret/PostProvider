namespace PostProvider.Models;

public record class Row
{
    public Row() 
    {
        CreatedOn = DateTime.Now;
    }

    public Row(string id, bool isPublished, string name) : this() 
    {
        Id = id;
        IsPublished = isPublished;
        Name = name;
    }

    public string Id { get; set; } = string.Empty;
    public bool IsPublished { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Key => Id;
    public DateTimeOffset CreatedOn { get; set; }
}