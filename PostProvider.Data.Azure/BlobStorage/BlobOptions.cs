namespace PostProvider.Data.Azure.BlobStorage;

public class BlobOptions
{
    public const string Blobs = "Blobs";

    public string ContainerName { get; set; } = string.Empty;
}