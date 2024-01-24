using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace SourceGenerator;

public static class SourceGenerationHelper
{
    public const string Attribute =
        """
        namespace SourceGenerator
        {
            [System.AttributeUsage(System.AttributeTargets.Interface)]
            public class ApiSpecificationAttribute : System.Attribute
            {
            }
        }
        """;

    public static string GenerateExtensionInterface(ClassToGenerate classToGenerate)
    {
        return
            $$"""
              using System;
              using System.Threading.Tasks;
              using Microsoft.AspNetCore.Http;

              namespace Api;
              
              public interface I{{classToGenerate.Name}}
              {
              {{string.Join("\r\n", GenerateApiOperations(classToGenerate, GeneratingType.Interface).ToList())}}
              }
              """;
    }
    
    public static string GenerateMapRoute(ClassToGenerate classToGenerate)
    {
        return
            $$"""
              using Microsoft.AspNetCore.Builder;
              using Microsoft.AspNetCore.Http;
              using Microsoft.AspNetCore.Routing;

              namespace Api;

              public static class EndpointRouteBuilderExtensions{{classToGenerate.Name}}
              {
                  public static void Register{{classToGenerate.Name}}(this IEndpointRouteBuilder app)
                  {
              {{string.Join("\r\n", GenerateApiOperations(classToGenerate, GeneratingType.Mapper).ToList())}}
                  }
              }
              """;
    }

    private static IEnumerable<string> GenerateApiOperations(ClassToGenerate classToGenerate,
        GeneratingType generatingType)
    {
        return classToGenerate.ApiOperations
            .Where(x => x.GetAttributes().Any(y => y.AttributeClass?.BaseType?.Name == "HttpMethodAttribute"))
            .Select(apiOperation =>
            {
                var refitAttribute = apiOperation.GetAttributes()
                    .Where(x => x.AttributeClass?.BaseType?.Name == "HttpMethodAttribute")
                    .Where(x => x.AttributeClass != null)
                    .Select(x => new
                    {
                        Name = x.AttributeClass.Name.Replace("Attribute", string.Empty),
                        Path = x.ConstructorArguments.Single().Value
                    })
                    .Single();

                return generatingType switch
                {
                    GeneratingType.Interface => $"""
                                                     // {refitAttribute.Name}: {refitAttribute.Path}
                                                     Task<IResult> {apiOperation.Name}({string.Join(", ", GenerateArguments(apiOperation.Parameters).ToList())});
                                                 """,
                    GeneratingType.Mapper => $"""
                                                      app.Map{refitAttribute.Name}("{refitAttribute.Path}", (I{classToGenerate.Name} api, HttpRequest httpRequest)
                                                          => api.{apiOperation.Name}());
                                              """,
                    #pragma warning disable S3928
                    _ => throw new ArgumentOutOfRangeException(nameof(generatingType), generatingType, null)
                    #pragma warning restore
                };
            });
    }

    private static IEnumerable<string> GenerateArguments(ImmutableArray<IParameterSymbol> arguments)
    {
        return arguments.Select(argument => $"{argument.Type.Name} {argument.Name}");
    }
}