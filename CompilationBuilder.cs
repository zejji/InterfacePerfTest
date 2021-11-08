using BenchmarkDotNet.Attributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Diagnostics;
using System.Reflection;

namespace InterfacePerfTest
{
    internal class CompilationBuilder
    {
        public Compilation CreateCompilation(IEnumerable<string> sourceTexts)
        {
            var syntaxTrees = sourceTexts
                .Select(source => CSharpSyntaxTree.ParseText(source))
                .ToArray();
            
            var coreLibReference        = MetadataReference.CreateFromFile(typeof(Binder).GetTypeInfo().Assembly.Location);
            var netstandardReference    = MetadataReference.CreateFromFile(Assembly.Load("netstandard, Version=2.0.0.0").Location);
            var runtimeReference        = MetadataReference.CreateFromFile(Assembly.Load("System.Runtime, Version=6.0.0.0").Location);
            var systemReference         = MetadataReference.CreateFromFile(typeof(Console).GetTypeInfo().Assembly.Location);
            var benchmarkReference      = MetadataReference.CreateFromFile(typeof(BenchmarkAttribute).GetTypeInfo().Assembly.Location);
            var inProcessReference      = MetadataReference.CreateFromFile(typeof(InProcessAttribute).GetTypeInfo().Assembly.Location);

            var additionalReferences = new[] {
                coreLibReference,
                netstandardReference,
                runtimeReference,
                systemReference,
                benchmarkReference,
                inProcessReference,
            };

            var optimizationLevel = OptimizationLevel.Release;

            var compilation = CSharpCompilation.Create(
                assemblyName:   "compilation",
                syntaxTrees:    syntaxTrees,
                references:     additionalReferences,
                options:        new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                                    .WithOptimizationLevel(optimizationLevel));

            // Require that no errors occurred during compilation
            IEnumerable<Diagnostic> failures = compilation.GetDiagnostics();
            foreach (Diagnostic diagnostic in failures)
            {
                Debug.WriteLine("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
            }

            if (failures.Any()) throw new Exception("One or more errors occurred during compilation.");

            return compilation;
        }
    }
}
