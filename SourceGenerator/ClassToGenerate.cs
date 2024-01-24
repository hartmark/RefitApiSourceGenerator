using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace SourceGenerator;

public class ClassToGenerate(string name, IEnumerable<IMethodSymbol> apiOperations)
{
    public string Name { get; } = name;
    public IEnumerable<IMethodSymbol> ApiOperations { get; } = apiOperations;
}