using BenchmarkDotNet.Running;
using InterfacePerfTest;
using InterfacePerfTest.SourceCodeGenerators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Emit;
using System.Diagnostics;
using System.Reflection;

// See https://aka.ms/new-console-template for more information

var compilationBuilder = new CompilationBuilder();

List<string> sourceCodeFiles = new();

const long methodCount = 1;
var generator1 = new ContainerGenerator(methodCount);
var generator2 = new ContainerWithInterfacesGenerator(methodCount);

sourceCodeFiles.Add(generator1.GetContainerSourceText());
sourceCodeFiles.Add(generator2.GetContainerSourceText());

sourceCodeFiles.Add(@"
using BenchmarkDotNet.Attributes;
using MyCode;

namespace MyBenchMarks
{
    [InProcess]
    public class BenchmarkClass
    {
        [Benchmark]
        public void CallSayHello() 
        {
            var testObject = new HelloWorld();
            testObject.SayHello();
        }

        [Benchmark]
        public void CallSayHello2() 
        {
            var testObject = new HelloWorld2();
            testObject.SayHello2();
        }
    }
}
");

var compilation = compilationBuilder.CreateCompilation(sourceCodeFiles);

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

Type? type = assembly.GetType("MyBenchMarks.BenchmarkClass");

var summary = BenchmarkRunner.Run(type);
Debug.WriteLine(summary);