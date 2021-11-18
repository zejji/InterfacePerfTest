using System.Text;

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
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(@"
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
");
            return stringBuilder.ToString();
        }

        public string GetCallingCodeSourceText()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(@"
namespace MyCode
{
    public static class Caller
    {
        public static void BenchmarkMe() 
        {
            var testObject = new HelloWorld();
            testObject.SayHello();
        }
    }
}
");
            return stringBuilder.ToString();
        }
    }
}
