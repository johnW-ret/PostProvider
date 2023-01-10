using PostProvider.Models;
using PostProvider.Routes;
using PostProvider.Data.Azure;
using PostProvider.Data.Azure.AzureTableStorage;
using PostProvider.Data.Services;
using Microsoft.AspNetCore.Mvc;
using PostProvider;

var builder = WebApplication.CreateBuilder(args);

var tablesSection = builder.Configuration.GetSection("Tables");

builder.Services.AddPosts(tablesSection,
    options => options
    .AddAzureStorage(azOptions => azOptions
        .SetTableClientConfigurationSection(tablesSection)
        .SetBlobsClientConfigurationSection(builder.Configuration.GetSection("Blobs"))));

var app = builder.Build();

app.MapGet("/", () => "Hello 👋");

app.MapPostProviderRoutes();

app.Run();
