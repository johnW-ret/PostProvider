namespace PostProvider.Models;

public record class Post
{
    public Post(string url, string name, DateTimeOffset createdOn, string content)
    {
        Url = url;
        Name = name;
        CreatedOn = createdOn;
        Content = content;
    }

    public string Url { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTimeOffset CreatedOn { get; set; }
}