namespace PostProvider.Models;

public record class Post
{
    public Post(string url, string name, string content)
    {
        Url = url;
        Name = name;
        Content = content;
    }

    public string Url { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}