﻿using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.InProcess.Emit;
using InterfacePerfTest;
using System;
using System.Diagnostics;

// See https://aka.ms/new-console-template for more information

// Create an in-memory assembly.
var assembly = AssemblyBuilder.GetBenchmarkAssembly();

// Get the class with the benchmark annotations.
const string benchmarkClass = "MyBenchmarks.BenchmarkClass";
Type? type = assembly.GetType(benchmarkClass);
if (type == null) throw new InvalidOperationException($"\"{nameof(benchmarkClass)}\" could not be found.");

// Run the benchmarks.
var benchmarkConfig = DefaultConfig.Instance
    .AddDiagnoser(MemoryDiagnoser.Default)
    .AddJob(Job.ShortRun
        .WithLaunchCount(1)
        .WithWarmupCount(3)
        .WithUnrollFactor(16)
        .WithInvocationCount(16)
        .WithToolchain(InProcessEmitToolchain.Instance)
        .WithId("InProcess"));

var summary = BenchmarkRunner.Run(type, benchmarkConfig);

// Print results.
Debug.WriteLine(summary);