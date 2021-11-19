using InterfacePerfTest.SourceCodeBuilders;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace InterfacePerfTest
{
    internal static class AssemblyBuilder
    {
        public static Assembly GetBenchmarkAssembly(int methodCount, bool printSource = false)
        {
            const int maxPrintableMethods = 10;
            if (printSource && methodCount > maxPrintableMethods)
            {
                throw new InvalidOperationException($"{nameof(printSource)} can only be set to true when {nameof(methodCount)} is less than {maxPrintableMethods}, as it is intended to help with understanding the benchmark only.");
            }

            var methodCallOrder = Enumerable.Range(0, methodCount).ToList();
            methodCallOrder.Shuffle();

            List<string> sourceCodeFiles = new();

            var benchmarkClass = _getBenchmarkClass();
            sourceCodeFiles.Add(benchmarkClass);

            List<IContainerSourceCodeBuilder> builders = _getSourceCodeBuilders(methodCount, methodCallOrder);
            sourceCodeFiles.AddRange(_getSourceCodeFromBuilders(builders));

            if (printSource)
            {
                Console.WriteLine("The following source code will be benchmarked:");
                var asterisks = new string('*', 10);
                int i = 1;
                foreach (var file in sourceCodeFiles)
                {
                    //Console.WriteLine(asterisks + $" File {i++} " + asterisks);
                    Console.Write(file);
                }
            }

            var compilation = CompilationBuilder.CreateCompilation(sourceCodeFiles);
            var assembly = _getAssemblyFromCompilation(compilation);

            return assembly;
        }

        private static List<IContainerSourceCodeBuilder> _getSourceCodeBuilders(int methodCount, IEnumerable<int> methodCallOrder)
        {
            List<IContainerSourceCodeBuilder> builders = new()
            {
                new ContainerNoInterfacesSourceCodeBuilder(methodCount, methodCallOrder),
                new ContainerWithInterfacesSourceCodeBuilder(methodCount, methodCallOrder)
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
namespace MyBenchmarks
{
    using BenchmarkDotNet.Attributes;
    using MyCode;    

    public class BenchmarkClass
    {
        [Benchmark(OperationsPerInvoke = 4)]
        public long NoInterfaceBenchmark() 
        {
            return NoInterfaceContainerCaller.BenchmarkMe();
        }

        [Benchmark(OperationsPerInvoke = 4)]
        public long WithInterfacesBenchmark() 
        {
            return WithInterfacesContainerCaller.BenchmarkMe();
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
