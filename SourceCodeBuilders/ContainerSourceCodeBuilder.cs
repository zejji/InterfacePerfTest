namespace InterfacePerfTest.SourceCodeBuilders
{
    internal class ContainerSourceCodeBuilder : IContainerSourceCodeBuilder
    {
        private long _methodCount;

        public ContainerSourceCodeBuilder(long methodCount)
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
