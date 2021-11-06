using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection;

namespace InterfacePerfTest
{
    internal class CompilationBuilder
    {
        public Compilation CreateCompilation()
        {
            var source = _createTestSource();
            
            var coreLibReference = MetadataReference.CreateFromFile(typeof(Binder).GetTypeInfo().Assembly.Location);
            var netstandardReference = MetadataReference.CreateFromFile(Assembly.Load("netstandard, Version=2.0.0.0").Location);
            var runtimeReference = MetadataReference.CreateFromFile(Assembly.Load("System.Runtime, Version=6.0.0.0").Location);
            var systemReference = MetadataReference.CreateFromFile(typeof(Console).GetTypeInfo().Assembly.Location);

            var additionalReferences = new[] { coreLibReference, netstandardReference, runtimeReference, systemReference };

            var compilation = CSharpCompilation.Create(
                assemblyName: "compilation",
                syntaxTrees: new[] { CSharpSyntaxTree.ParseText(source) },
                references: additionalReferences,
                options: new CSharpCompilationOptions(OutputKind.ConsoleApplication));

            // Require that no serious errors occurred during compilation
            IEnumerable<Diagnostic> failures = _getErrorDiagnostics(compilation);
            foreach (Diagnostic diagnostic in failures)
            {
                Debug.WriteLine("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
            }

            if (failures.Any()) throw new Exception();

            return compilation;
        }

        private static string _createTestSource()
        {
            return @"
using System;
namespace HelloWorld
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var myClass = new HelloWorld();
            myClass.SayHello();
        }
    }

    public class HelloWorld
    {
        public void SayHello() 
        {
            Console.WriteLine(""Hello from generated code!"");
        }
    }
}
";
        }

        private static IEnumerable<Diagnostic> _getErrorDiagnostics(Compilation inputCompilation)
        {
            ImmutableArray<Diagnostic> compilationDiagnostics = inputCompilation.GetDiagnostics();

            IEnumerable<Diagnostic> failures = compilationDiagnostics.Where(diagnostic =>
                        diagnostic.Id != "CS5001" && // i.e. "Program does not contain a static 'Main' method suitable for an entry point"
                        (diagnostic.IsWarningAsError ||
                        diagnostic.Severity == DiagnosticSeverity.Error));

            return failures;
        }
    }
}
