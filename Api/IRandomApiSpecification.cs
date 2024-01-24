using System.Threading.Tasks;
using Refit;
using SourceGenerator;
// ReSharper disable UnusedMember.Global

namespace Api;

[ApiSpecification]
public interface IRandomApiSpecification
{
    [Get("/api/randomInt")]
    Task<IApiResponse<int>> GetRandomInt();
    
    [Post("/api/randomInt")]
    Task<IApiResponse> PostRandomInt();
    
    // TODO: We now only supports without parameters
    // Task<IApiResponse> PostRandomInt(int value);
}