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

    public static RouteGroupBuilder MapReadOnlyPostApis(this RouteGroupBuilder group)
    {
        group.MapGet("/", async (string key, IPostsTableAccess tableAccess, IPostClient postClient)
            => await tableAccess.GetRow(key) switch
            {
                Row row => await postClient.GetPost(row.Name) switch
                {
                    Post post => Results.Ok(post),
                    null => Results.Problem(),
                },
                _ => Results.NotFound()
            });

        group.MapGet("/all", async (string? continuationToken, IPostsTableAccess tableAccess, IPostClient postClient) =>
        {
            try
            {
                return Results.Ok((await tableAccess
                    .GetRows("PartitionKey ne 'null'", continuationToken))
                    .Select(async r => await postClient.GetPost(r.Name))
                    .Select(t => t.Result)
                    .OfType<Post>()
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
                    .OrderByDescending(r => r.CreatedOn));
            }
            catch (Exception)
            {
                return Results.Problem();
            }
        });

        return group;
    }

    public static RouteGroupBuilder MapWritePostApis(this RouteGroupBuilder group)
    {
        group.MapPost("/", async (
                [FromBody] PostInputs postInputs,
                IPostsTableAccess tableAccess,
                IPostClient postClient)
            => await postClient.CreatePost(postInputs) switch
            {
                ({ Url: not null, Name: not null } newPost, HttpStatusCode.Created)
                    => await tableAccess.AddRow(new(newPost.Url, newPost.Name)) switch
                    {
                        (not null, HttpStatusCode.NoContent) => Results.NoContent(),
                        (_, var code) => Results.Problem($"{(int)code} {code}")
                    },
                (_, var code) => Results.Problem($"{(int)code} {code}")
            });

        group.MapPut("/", async (
                [FromBody] PostInputs postInputs,
                IPostsTableAccess tableAccess,
                IPostClient postClient)
            => await tableAccess.GetRow(postInputs.Name) switch
            {
                Row row => (await postClient.PutPost(postInputs)).StatusCode switch
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
        .MapReadOnlyPostApis()
        .MapWritePostApis();
}
