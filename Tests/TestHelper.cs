using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Refit;
using SourceGenerator;
using Xunit.Abstractions;

namespace Tests;

public static class TestHelper
{
    public static Task VerifyRouteSourceGenerator(string source, ITestOutputHelper testOutputHelper)
    {
        // Parse the provided string into a C# syntax tree
        var syntaxTree = CSharpSyntaxTree.ParseText(source);

        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var references =
            (from assembly in assemblies
                where !assembly.IsDynamic && !string.IsNullOrWhiteSpace(assembly.Location)
                select MetadataReference.CreateFromFile(assembly.Location)).Cast<MetadataReference>().ToList();

        references.Add(MetadataReference.CreateFromFile(typeof(GetAttribute).Assembly.Location));

        // Create a Roslyn compilation for the syntax tree.
        var compilation = CSharpCompilation.Create(
            assemblyName: "Tests",
            syntaxTrees: new[] { syntaxTree },
            references: references,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var compileDiagnostics = compilation.GetDiagnostics();

        testOutputHelper.WriteLine(string.Join(Environment.NewLine,
            compileDiagnostics
                .GroupBy(x => x.Severity)
                .OrderByDescending(x => x.Key)
                .Select(x =>
                    $"{x.Key}:{Environment.NewLine}" +
                    $"{string.Join(Environment.NewLine, x.Select(y => $"    {y.Id}: {y.GetMessage()}"))}")));

        Assert.Empty(compileDiagnostics.Select(x => x.GetMessage()));


        // Create an instance of our RouteSourceGenerator incremental source generator
        var generator = new RouteSourceGenerator();

        // The GeneratorDriver is used to run our generator against a compilation
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        // Run the source generator!
        driver = driver.RunGenerators(compilation);

        // Use verify to snapshot test the source generator output!
        return Verify(driver.GetRunResult())
            .UseDirectory("Snapshots");
    }
}