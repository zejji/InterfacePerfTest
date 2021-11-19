using System.Text;

namespace InterfacePerfTest.SourceCodeBuilders
{
    internal class ContainerSourceCodeBuilder : IContainerSourceCodeBuilder
    {
        private int _methodCount;
        private IEnumerable<int> _methodCallOrder;

        public ContainerSourceCodeBuilder(int methodCount, IEnumerable<int> methodCallOrder)
        {
            _methodCount = methodCount;
            _methodCallOrder = methodCallOrder;
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
        public static long BenchmarkMe() 
        {
            var testObject = new HelloWorld();
            testObject.SayHello();
            return 0L;
        }
    }
}
");
            return stringBuilder.ToString();
        }
    }
}
