using System.Runtime.CompilerServices;

namespace Tests;

public static class Initializer
{
    [ModuleInitializer]
    public static void Init() =>
        VerifySourceGenerators.Initialize();
}