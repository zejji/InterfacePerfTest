using BenchmarkDotNet.Running;
using InterfacePerfTest;
using System.Diagnostics;

// See https://aka.ms/new-console-template for more information

// Create in-memory assembly
var assembly = AssemblyBuilder.GetBenchmarkAssembly();
Type? type = assembly.GetType("MyBenchMarks.BenchmarkClass");

// Run benchmarks
var summary = BenchmarkRunner.Run(type);

// Print results
Debug.WriteLine(summary);