using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace SourceGenerator;

[Generator]
public class RouteSourceGenerator : IIncrementalGenerator
{
    /// <inheritdoc/>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx =>
            ctx.AddSource(
                "RefitExtensionAttribute.g.cs",
                SourceText.From(SourceGenerationHelper.Attribute, Encoding.UTF8)));

        var interfaceDeclarations = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                "SourceGenerator.ApiSpecificationAttribute",
                predicate: (x, _) => x is InterfaceDeclarationSyntax,
                transform: GetClassToGenerate)
            .Collect();

        context.RegisterSourceOutput(interfaceDeclarations, GenerateSource);
    }

    private static ClassToGenerate GetClassToGenerate(GeneratorAttributeSyntaxContext context,
        CancellationToken cancellationToken)
    {
        var currentInterface = (InterfaceDeclarationSyntax)context.TargetNode;

        if (context.SemanticModel.GetDeclaredSymbol(currentInterface, cancellationToken) is not ITypeSymbol typeSymbol)
        {
            return null;
        }

        var methods = new List<IMethodSymbol>();

        foreach (var member in typeSymbol.GetMembers())
        {
            if (member is IMethodSymbol method)
            {
                methods.Add(method);
            }
        }

        var name = new string(typeSymbol.Name.Skip(1).ToArray()).Replace("Specification", string.Empty);
        return new ClassToGenerate(name, methods);
    }

    private void GenerateSource(SourceProductionContext context, ImmutableArray<ClassToGenerate> typeSymbols)
    {
        foreach (var classToGenerate in typeSymbols)
        {
            var generatedInterface = SourceGenerationHelper.GenerateExtensionInterface(classToGenerate);
            context.AddSource($"RefitExtension.I{classToGenerate.Name}.g.cs", SourceText.From(generatedInterface, Encoding.UTF8));

            var generatedMapper = SourceGenerationHelper.GenerateMapRoute(classToGenerate);
            context.AddSource($"RefitExtension.{classToGenerate.Name}Mapper.g.cs", SourceText.From(generatedMapper, Encoding.UTF8));

        }
    }
}