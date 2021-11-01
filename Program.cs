using BenchmarkDotNet.Running;
using InterfacePerfTest;
using System.Diagnostics;

// See https://aka.ms/new-console-template for more information
var summary = BenchmarkRunner.Run<Md5VsSha256>();
Debug.WriteLine(summary);
