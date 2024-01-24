using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Api.Apis;

public class RandomApi : IRandomApi
{
    public Task<IResult> GetRandomInt()
    {
        return Task.FromResult(Results.Ok(new Random().Next()));
    }

    public Task<IResult> PostRandomInt()
    {
        return Task.FromResult(Results.NoContent());
    }
}
