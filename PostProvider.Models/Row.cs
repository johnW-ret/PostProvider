namespace PostProvider.Models;

public class Row
{
    public Row() 
    {
        CreatedOn = DateTime.Now;
    }

    public string Url { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateTimeOffset CreatedOn { get; set; }
}