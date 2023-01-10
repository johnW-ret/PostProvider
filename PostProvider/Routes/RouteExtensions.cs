using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using PostProvider.Data.Services;
using PostProvider.Models;

namespace PostProvider.Routes;

public static class RouteExtensions
{
    private const string DefaultPostRoute = "post";

    public static RouteGroupBuilder MapPostApis(this RouteGroupBuilder group)
    {
        group.MapGet("/", ([FromQuery] string key, IPostsTableAccess postAccess)
            => postAccess.GetRow(key));

        group.MapPost("/add", ([FromBody] Row row, IPostsTableAccess postAccess)
            => postAccess.AddRow(row));

        return group;
    }

    public static IEndpointRouteBuilder MapPostProviderRoutes(this IEndpointRouteBuilder routeBuilder, string routeName = DefaultPostRoute)
    {
        routeBuilder.MapGroup(routeName).MapPostApis();

        return routeBuilder;
    }
}
