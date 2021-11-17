using BenchmarkDotNet.Running;
using InterfacePerfTest;
using System.Diagnostics;

// See https://aka.ms/new-console-template for more information

// Create an in-memory assembly.
var assembly = AssemblyBuilder.GetBenchmarkAssembly();

// Get the class with the benchmark annotations.
const string benchmarkClass = "MyBenchmarks.BenchmarkClass";
Type? type = assembly.GetType(benchmarkClass);
if (type == null) throw new InvalidOperationException($"\"{nameof(benchmarkClass)}\" could not be found.");

// Run the benchmarks.
var summary = BenchmarkRunner.Run(type);

// Print results.
Debug.WriteLine(summary);