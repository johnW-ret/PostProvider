namespace PostProvider;

public class PostStorageOptions
{
    public Dictionary<string, HashSet<(Type, Type)>> Implementations { get; set; } = new();
}