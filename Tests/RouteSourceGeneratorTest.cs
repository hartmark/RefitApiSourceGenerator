using SourceGenerator;
using Xunit.Abstractions;

namespace Tests;

public class RouteSourceGeneratorTest(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public Task VerifyRouteSourceGenerator()
    {
        // Build up the source code
        const string source =
            $$"""
            using System.Threading.Tasks;
            using Refit;
            using SourceGenerator;
            
            {{SourceGenerationHelper.Attribute}}
            
            namespace Api
            {
            [ApiSpecification]
            public interface IRandomApiSpecification
            {
                [Get("/api/randomInt")]
                Task<IApiResponse<int>> GetRandomInt();
            }
            }
            """;
        return TestHelper.VerifyRouteSourceGenerator(source, testOutputHelper);
    }
}