using System.Net;

namespace PostProvider.Data.Services;

public record TResponse<T>(T? Value, HttpStatusCode StatusCode);