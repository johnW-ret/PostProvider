using Azure;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;
using PostProvider.Data.Services;
using PostProvider.Models;
using System.Net;
using System.Text.Json;

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
    {
        Guid guid = Guid.NewGuid();

        return blobContainerClient
            .UploadBlobAsync(guid.ToString(), new BinaryData(postInputs).ToStream())
            .ContinueWith(t => new Func<Response, TResponse<Post>>(
            r => new(
                    Value: new(
                        guid,
                        isPublished: postInputs.IsPublished,
                        url: blobContainerClient.Uri.ToString() + "/" + guid,
                        name: postInputs.Name,
                        createdOn: DateTimeOffset.Now,
                        content: postInputs.Content),
                    StatusCode: (HttpStatusCode)r.Status))
            (t.Result.GetRawResponse()));
    }

    public async Task<bool> DeletePost(string key)
    {
        try
        {
            return await blobContainerClient.DeleteBlobIfExistsAsync(key);
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<TResponse<Post>> PutPost(string key, PostInputs postInputs)
    {
        var client = blobContainerClient.GetBlobClient(key);
        Guid guid = Guid.NewGuid();

        bool exists = await client.ExistsAsync();

        return await client
            .UploadAsync(new BinaryData(postInputs).ToStream(), exists)
            .ContinueWith(t => new Func<Response, TResponse<Post>>(
                r => new(
                    Value: new(
                        guid,
                        isPublished: postInputs.IsPublished,
                        url: blobContainerClient.Uri.ToString() + "/" + postInputs.Name,
                        name: postInputs.Name,
                        createdOn: DateTimeOffset.Now,
                        content: postInputs.Content),
                    StatusCode: (HttpStatusCode)r.Status))
            (t.Result.GetRawResponse()));
    }

    public async Task<Post?> GetPost(string key)
    {
        try
        {
            Guid guid = Guid.Parse(key);

            var client = blobContainerClient
                .GetBlobClient(key);

            if (await client
                .DownloadContentAsync()
                .ContinueWith(downloadResult =>
                    JsonSerializer.Deserialize<PostInputs>(downloadResult.Result.Value.Content.ToString())
                ) is not PostInputs blob)
                return null;

            var post = (Post?)new Post(
                    guid,
                    isPublished: blob.IsPublished,
                    client.Uri.ToString(),
                    blob.Name,
                    (await client.GetPropertiesAsync()).Value.CreatedOn,
                    blob.Content);

            return post;
        }
        catch (Exception)
        {
            return null;
        }
    }
}
