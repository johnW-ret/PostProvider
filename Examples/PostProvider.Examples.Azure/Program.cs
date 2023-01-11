using PostProvider.Data.Azure.AzureTableStorage;
using PostProvider;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPosts(
    options => options.AddAzureStorage(
        builder.Configuration.GetSection("Azure")));

var app = builder.Build();

app.MapGet("/", () => "Hello 👋");

app.MapPostProviderRoutes();

app.Run();
