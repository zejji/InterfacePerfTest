namespace InterfacePerfTest.SourceCodeGenerators
{
    internal class ContainerGenerator : IContainerGenerator
    {
        private long _methodCount;

        public ContainerGenerator(long methodCount)
        {
            _methodCount = methodCount;
        }

        public string GetContainerSourceText()
        {
            return @"
using System;

namespace MyCode
{
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

        public string GetCallingCodeSourceText()
        {
            return @"
namespace MyCode
{
    public static class Caller
    {
        public static void Run() 
        {
            var testObject = new HelloWorld();
            testObject.SayHello();
        }
    }
}
";
        }
    }
}
