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

    public static class StrongInjectContainerExtensions
    {
	    public static TResult Run<T, TResult, TParam>(this IContainer<T> container, Func<T, TParam, TResult> func, TParam param)
	    {
		    return container.Run(func, param);
	    }

	    public static TResult Run<T, TResult>(this IContainer<T> container, Func<T, TResult> func)
	    {
		    return container.Run((T t, Func<T, TResult> func) => func(t), func);
	    }

	    public static void Run<T>(this IContainer<T> container, Action<T> action)
	    {
		    container.Run<object, Action<T>>((Func<T, Action<T>, object>)delegate (T t, Action<T> action)
		    {
			    action(t);
			    return null;
		    }, action);
	    }
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
        public static void BenchmarkMe() 
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
