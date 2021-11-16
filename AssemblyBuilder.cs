using InterfacePerfTest.SourceCodeBuilders;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Emit;
using System.Reflection;

namespace InterfacePerfTest
{
    internal static class AssemblyBuilder
    {
        public static Assembly GetBenchmarkAssembly()
        {
            List<IContainerSourceCodeBuilder> builders = _getSourceCodeBuilders();
            List<string> sourceCodeFiles = _getSourceCodeFromBuilders(builders);

            var benchmarkClass = _getBenchmarkClass();
            sourceCodeFiles.Add(benchmarkClass);

            var compilation = CompilationBuilder.CreateCompilation(sourceCodeFiles);
            var assembly = _getAssemblyFromCompilation(compilation);

            return assembly;
        }

        private static List<IContainerSourceCodeBuilder> _getSourceCodeBuilders()
        {
            const long methodCount = 1;
            List<IContainerSourceCodeBuilder> builders = new()
            {
                new ContainerSourceCodeBuilder(methodCount),
                new ContainerWithInterfacesSourceCodeBuilder(methodCount)
            };
            return builders;
        }

        private static List<string> _getSourceCodeFromBuilders(List<IContainerSourceCodeBuilder> builders)
        {
            List<string> sourceCodeFiles = new();
            foreach (var builder in builders)
            {
                sourceCodeFiles.Add(builder.GetContainerSourceText());
                sourceCodeFiles.Add(builder.GetCallingCodeSourceText());
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
