using Azure;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;
using PostProvider.Data.Services;
using PostProvider.Models;
using System.Net;

namespace PostProvider.Data.Azure.BlobStorage;

public class AzureBlobStoragePostClient : IPostClient
{
    private readonly BlobContainerClient blobContainerClient;
    private readonly BlobOptions blobOptions;

    public AzureBlobStoragePostClient(
        BlobServiceClient blobServiceClient,
        IOptions<BlobOptions> options)
    {
        blobOptions = options.Value;

        blobContainerClient = blobServiceClient.GetBlobContainerClient(blobOptions.ContainerName);
    }

    public Task<TResponse<Post>> CreatePost(Post post)
        => blobContainerClient
                .UploadBlobAsync(post.Name, new BinaryData(post.Content).ToStream())
                .ContinueWith(t => new Func<Response, TResponse<Post>>(
                    r => new(
                        post with { Url = blobContainerClient.Uri.ToString() + "/" + post.Name },
                        (HttpStatusCode)r.Status))
                (t.Result.GetRawResponse()));

    public Task<Post?> GetPost(string name)
    {
        var client = blobContainerClient
            .GetBlobClient(name);

        return Task.Run(async () =>
            (Post?)new Post(
                client.Uri.ToString(),
                client.Name,
                await client
                    .DownloadContentAsync()
                    .ContinueWith(downloadResult =>
                        downloadResult.Result.Value.Content.ToString()
                )));
    }
}
