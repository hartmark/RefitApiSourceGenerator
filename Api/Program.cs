using Api;
using Api.Apis;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<IRandomApi, RandomApi>();

var app = builder.Build();
app.UseSwagger();

app.AddGeneratedRoutes();

app.UseSwaggerUI(c =>
{
    c.RoutePrefix = string.Empty;
    c.SwaggerEndpoint("./swagger/v1/swagger.json", "Api");
    c.DocumentTitle = "Api - Swagger";
});

app.Run();
