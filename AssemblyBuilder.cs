using InterfacePerfTest.SourceCodeGenerators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Emit;
using System.Reflection;

namespace InterfacePerfTest
{
    internal static class AssemblyBuilder
    {
        public static Assembly GetBenchmarkAssembly()
        {
            List<IContainerGenerator> generators = _getGenerators();
            List<string> sourceCodeFiles = _getSourceCodeFromGenerators(generators);

            var benchmarkClass = _getBenchmarkClass();
            sourceCodeFiles.Add(benchmarkClass);

            var compilation = CompilationBuilder.CreateCompilation(sourceCodeFiles);
            var assembly = _getAssemblyFromCompilation(compilation);

            return assembly;
        }

        private static List<IContainerGenerator> _getGenerators()
        {
            const long methodCount = 1;
            List<IContainerGenerator> generators = new()
            {
                new ContainerGenerator(methodCount),
                new ContainerWithInterfacesGenerator(methodCount)
            };
            return generators;
        }

        private static List<string> _getSourceCodeFromGenerators(List<IContainerGenerator> generators)
        {
            List<string> sourceCodeFiles = new();
            foreach (var generator in generators)
            {
                sourceCodeFiles.Add(generator.GetContainerSourceText());
                sourceCodeFiles.Add(generator.GetCallingCodeSourceText());
            }

            return sourceCodeFiles;
        }

        private static string _getBenchmarkClass()
        {
            return @"
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
            Caller.Run();
        }

        [Benchmark]
        public void CallSayHello2() 
        {
            Caller2.Run();
        }
    }
}
";
        }

        private static Assembly _getAssemblyFromCompilation(Compilation compilation)
        {
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

            return assembly;
        }
    }
}
