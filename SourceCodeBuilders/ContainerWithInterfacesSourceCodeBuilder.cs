using System.Text;

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
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(@"
using System;

namespace StrongInject
{
    public interface IContainer<T> : IDisposable
    {
        TResult Run<TResult, TParam>(Func<T, TParam, TResult> func, TParam param);
    }
}

namespace MyCode
{
    public class HelloWorld2
    {
");
            for (long i = 0; i < _methodCount; i++) {
                stringBuilder.Append(@"
        public void SayHello2() 
        {
            Console.WriteLine(""Hello from generated code2!"");
        }
");
            }

            stringBuilder.Append(@"
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
    public static class Caller2
    {
        public static void Run() 
        {
            var testObject = new HelloWorld2();
            testObject.SayHello2();
        }
    }
}
");
            return stringBuilder.ToString();
        }
    }
}
