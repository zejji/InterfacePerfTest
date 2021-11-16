namespace InterfacePerfTest.SourceCodeBuilders
{
    internal class ContainerWithInterfacesSourceCodeBuilder : IContainerSourceCodeBuilder
    {
        private long _methodCount;

        public ContainerWithInterfacesSourceCodeBuilder(long methodCount)
        {
            _methodCount = methodCount;
        }

        public string GetContainerSourceText()
        {
            return @"
using System;

namespace MyCode
{
    public class HelloWorld2
    {
        public void SayHello2() 
        {
            Console.WriteLine(""Hello from generated code2!"");
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
    public static class Caller2
    {
        public static void Run() 
        {
            var testObject = new HelloWorld2();
            testObject.SayHello2();
        }
    }
}
";
        }
    }
}
