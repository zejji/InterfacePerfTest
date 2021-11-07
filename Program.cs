using BenchmarkDotNet.Running;
using InterfacePerfTest;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Emit;
using System.Diagnostics;
using System.Reflection;

//// See https://aka.ms/new-console-template for more information
//var summary = BenchmarkRunner.Run<Md5VsSha256>();
//Debug.WriteLine(summary);

var compilationBuilder = new CompilationBuilder();
var compilation = compilationBuilder.CreateCompilation();

Assembly? assembly = null;

// Create an in-memory assembly from the compilation.
// https://www.tugberkugurlu.com/archive/compiling-c-sharp-code-into-memory-and-executing-it-with-roslyn
using (var ms = new MemoryStream())
{
    EmitResult result = compilation.Emit(ms);

    if (!result.Success)
    {
        IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
            diagnostic.IsWarningAsError ||
            diagnostic.Severity == DiagnosticSeverity.Error);

        foreach (Diagnostic diagnostic in failures)
        {
            Console.Error.WriteLine("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
        }
    }
    else
    {
        ms.Seek(0, SeekOrigin.Begin);
        assembly = Assembly.Load(ms.ToArray());
    }
}

if (assembly == null)
{
    throw new InvalidOperationException($"{nameof(assembly)} is null.");
}

Type? type = assembly.GetType("HelloWorld.HelloWorld");

var summary = BenchmarkRunner.Run(type);
Debug.WriteLine(summary);