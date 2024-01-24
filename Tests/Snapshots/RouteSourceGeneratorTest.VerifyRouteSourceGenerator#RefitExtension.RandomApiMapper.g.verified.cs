//HintName: RefitExtension.RandomApiMapper.g.cs
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Api;

public static class EndpointRouteBuilderExtensionsRandomApi
{
    public static void RegisterRandomApi(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/randomInt", (IRandomApi api, HttpRequest httpRequest)
            => api.GetRandomInt());
    }
}