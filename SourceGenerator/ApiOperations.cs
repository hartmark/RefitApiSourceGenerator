using System.Collections.Generic;

namespace SourceGenerator;

public class ApiOperations(
    string name,
    string httpVerb,
    string path,
    string returnTypeName,
    List<(string ArgumentType, string Name)> arguments)
{
    public string Name { get; set; } = name;
    public string HttpVerb { get; } = httpVerb;
    public string Path { get; } = path;
    public string ReturnTypeName { get; } = returnTypeName;
    public List<(string ArgumentType, string Name)> Arguments { get; set; } = arguments;
}