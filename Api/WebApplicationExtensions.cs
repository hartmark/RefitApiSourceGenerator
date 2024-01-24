using Microsoft.AspNetCore.Routing;

namespace Api;

public static class WebApplicationExtensions
{
    public static void AddGeneratedRoutes(this IEndpointRouteBuilder app)
    {
        // TODO: Automagically add this
        app.RegisterRandomApi();
    }
}