using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using PostProvider.Data;
using PostProvider.Data.Services;
using PostProvider.Models;
using System.Net;

namespace PostProvider;

public static class RouteExtensions
{
    private const string DefaultPostRoute = "post";

    public static RouteGroupBuilder MapReadOnlyPostApis(this RouteGroupBuilder group, bool isEditor = false)
    {
        group.MapGet("/", async (string key, IPostsTableAccess tableAccess, IPostClient postClient)
            => await tableAccess.GetRow(key) switch
            {
                Row row => await postClient.GetPost(row.Key) switch
                {
                    { IsPublished: bool p } post when isEditor || p => Results.Ok(post),
                    _ => Results.Problem(),
                },
                _ => Results.NotFound()
            });

        group.MapGet("/all", async (string? continuationToken, IPostsTableAccess tableAccess, IPostClient postClient) =>
        {
            try
            {
                return Results.Ok((await tableAccess
                    .GetRows("PartitionKey ne 'null'", continuationToken))
                    .Select(async r => await postClient.GetPost(r.Id))
                    .Select(t => t.Result)
                    .OfType<Post>()
                    .Where(p => isEditor || p.IsPublished)
                    .OrderByDescending(p => p.CreatedOn));
            }
            catch (Exception)
            {
                return Results.Problem();
            }
        });

        group.MapGet("/metadata", async (string? continuationToken, IPostsTableAccess tableAccess, IPostClient postClient) =>
        {
            try
            {
                return Results.Ok((await tableAccess
                    .GetRows("PartitionKey ne 'null'", continuationToken))
                    .Where(p => isEditor || p.IsPublished)
                    .OrderByDescending(r => r.CreatedOn));
            }
            catch (Exception)
            {
                return Results.Problem();
            }
        });

        return group;
    }

    public static RouteGroupBuilder MapEditorPostApis(this RouteGroupBuilder group)
    {
        group.MapReadOnlyPostApis(true);

        group.MapPost("/", async (
                [FromBody] PostInputs postInputs,
                IPostsTableAccess tableAccess,
                IPostClient postClient)
            => await postClient.CreatePost(postInputs) switch
            {
                ({ Url: not null, Name: string name } newPost, HttpStatusCode.Created)
                    => await tableAccess.AddRow(new(newPost.Guid.ToString(), postInputs.IsPublished, newPost.Url, name)) switch
                    {
                        (Row row, HttpStatusCode.NoContent) => Results.Created($"/{row.Id}", null),
                        (_, var code) => Results.Problem($"{(int)code} {code}")
                    },
                (_, var code) => Results.Problem($"{(int)code} {code}")
            });

        group.MapPut("/", async (
                string key,
                [FromBody] PostInputs postInputs,
                IPostsTableAccess tableAccess,
                IPostClient postClient)
            => await tableAccess.GetRow(key)
                switch
                {
                    Row row => (await postClient.PutPost(key, postInputs)).StatusCode switch
                    {
                        HttpStatusCode.NoContent or HttpStatusCode.Created => Results.NoContent(),
                        var code => Results.Problem($"{(int)code} {code}")
                    },
                    _ => Results.NotFound()
                });

        group.MapDelete("/", async (
            string key,
            IPostsTableAccess tableAccess,
            IPostClient postClient)
        => await postClient.DeletePost(key) switch
        {
            true => await tableAccess.DeleteRow(key) switch
            {
                HttpStatusCode.NoContent => Results.NoContent(),
                var code => Results.Problem($"{(int)code} {code}")
            },
            false => Results.Problem()
        });

        return group;
    }

    public static RouteGroupBuilder MapPostProviderRoutes(
            this IEndpointRouteBuilder routeBuilder,
            string routeName = DefaultPostRoute)
        => routeBuilder
        .MapGroup(routeName)
        .MapEditorPostApis();
}
