//HintName: RefitExtension.IRandomApi.g.cs
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Api;

public interface IRandomApi
{
    // Get: /api/randomInt
    Task<IResult> GetRandomInt();
}