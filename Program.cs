using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.InProcess.Emit;
using InterfacePerfTest;
using System;
using System.Diagnostics;

Console.WriteLine("This application is intended to benchmark the cost of using a large number of explicit interface implementations in a container class.");
Console.WriteLine("It generates source code for two container implementations, one using explicit interface implementations for each 'Run' method, and one using standard class methods.");
Console.WriteLine("It then benchmarks these two implementations by calling each 'Run' method in a random order. The same order is used for both containers.");
Console.WriteLine();

// Config.
Console.Write("Required number of methods (NB: you can only print source if <= 10, but use a high value such as 1000 (quick) or 5000 (quite slow) for proper benchmarking): ");
int methodCount = int.Parse(Console.ReadLine() ?? "");
const int maxMethods = 100000;
if (methodCount <= 0)
{
    throw new ArgumentOutOfRangeException($"{nameof(methodCount)} must be greater than 0.");
}
if (methodCount > maxMethods)
{
    throw new ArgumentOutOfRangeException($"{nameof(methodCount)} is greater than {maxMethods}. You really don't want to do this!");
}

bool printSource = false;
if (methodCount <= 10)
{
    Console.Write("Print source code before execution? (y/N): ");
    printSource = (Console.ReadLine() ?? "").ToLower().Trim() == "y";
}

Console.Write("Use shortened benchmark run (primarily intended for use during development)? (y/N): ");
bool shortRun = (Console.ReadLine() ?? "").ToLower().Trim() == "y";

Console.WriteLine($"Method count is {methodCount}. "
    + $"Print source is {(printSource ? "enabled" : "disabled")}. "
    + $"Short run is {(shortRun ? "enabled" : "disabled")}.");

// Create an in-memory assembly.
var assembly = AssemblyBuilder.GetBenchmarkAssembly(methodCount, printSource);

// Get the class with the benchmark annotations.
const string benchmarkClass = "MyBenchmarks.BenchmarkClass";
Type? type = assembly.GetType(benchmarkClass);
if (type == null) throw new InvalidOperationException($"\"{nameof(benchmarkClass)}\" could not be found.");

// Configure BenchmarkDotNet.
var benchmarkConfig = DefaultConfig.Instance
    .AddDiagnoser(MemoryDiagnoser.Default);

if (shortRun) {
    benchmarkConfig.AddJob(Job.ShortRun
        .WithLaunchCount(1)
        .WithWarmupCount(3)
        .WithUnrollFactor(16)
        .WithInvocationCount(16)
        .WithToolchain(InProcessEmitToolchain.Instance)
        .WithId("InProcess"));
}
else
{
    benchmarkConfig.AddJob(Job.MediumRun
        .WithToolchain(InProcessEmitToolchain.Instance)
        .WithId("InProcess"));
}

// Run the benchmarks.
var summary = BenchmarkRunner.Run(type, benchmarkConfig);

// Print results.
Debug.WriteLine(summary);