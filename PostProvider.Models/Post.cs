namespace PostProvider.Models;

public record class Post
{
    public Post(string guid, bool isPublished, string url, string name, DateTimeOffset createdOn, string content)
    {
        Guid = Guid.Parse(guid);
        IsPublished = isPublished;
        Url = url;
        Name = name;
        CreatedOn = createdOn;
        Content = content;
    }

    public Guid Guid { get; set; }
    public string Url { get; set; } = string.Empty;
    public bool IsPublished { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTimeOffset CreatedOn { get; set; }
}