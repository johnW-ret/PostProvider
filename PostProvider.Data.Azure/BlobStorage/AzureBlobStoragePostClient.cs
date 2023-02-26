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

    public Task<TResponse<Post>> CreatePost(PostInputs postInputs)
        => blobContainerClient
            .UploadBlobAsync(postInputs.Name, new BinaryData(postInputs.Content).ToStream())
            .ContinueWith(t => new Func<Response, TResponse<Post>>(
                r => new(
                    Value: new(
                        url: blobContainerClient.Uri.ToString() + "/" + postInputs.Name,
                        name: postInputs.Name,
                        createdOn: DateTimeOffset.Now,
                        content: postInputs.Content),
                    StatusCode: (HttpStatusCode)r.Status))
            (t.Result.GetRawResponse()));

    public async Task<Post?> GetPost(string name)
    {
        try
        {
            var client = blobContainerClient
                .GetBlobClient(name);

            var post = (Post?)new Post(
                    client.Uri.ToString(),
                    client.Name,
                    (await client.GetPropertiesAsync()).Value.CreatedOn,
                    await client
                        .DownloadContentAsync()
                        .ContinueWith(downloadResult =>
                            downloadResult.Result.Value.Content.ToString()
                    ));

            return post;
        }
        catch (Exception)
        {
            return null;
        }
    }
}
