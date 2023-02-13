using PostProvider.Data.Azure.AzureTableStorage;
using PostProvider;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPosts(
    options => options.AddAzureStorage(
        builder.Configuration.GetSection("Azure")));

// Configure Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.MapGet("/", () => "Hello 👋");

app.MapPostProviderRoutes();

app.Run();
