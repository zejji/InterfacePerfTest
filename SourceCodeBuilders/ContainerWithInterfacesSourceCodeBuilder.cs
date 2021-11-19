﻿using System.Text;

namespace InterfacePerfTest.SourceCodeBuilders
{
    internal class ContainerWithInterfacesSourceCodeBuilder : IContainerSourceCodeBuilder
    {
        private int _methodCount;
        private IEnumerable<int> _methodCallOrder;

        public ContainerWithInterfacesSourceCodeBuilder(int methodCount, IEnumerable<int> methodCallOrder)
        {
            _methodCount = methodCount;
            _methodCallOrder = methodCallOrder;
        }

        public string GetContainerSourceText()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(@"
namespace StrongInject
{
    using System;

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
    using StrongInject;
    using System;

    public static class MyStaticClass
    {
        public static int MyVal { get; set; } = 0;
    }
");
            for (int i = 0; i < _methodCount; i++)
            {
                stringBuilder.Append(@$"
    public class MyCommand{i}
    {{
        public int MyVal {{ get; set; }}
    }}

    public class MyCommandHandler{i}
    {{
        public void Handle(MyCommand{i} command)
        {{
            MyStaticClass.MyVal += command.MyVal;
        }}
    }}
");
            }

            stringBuilder.Append(@"
    public class ContainerWithInterfaces :
");

            for (int j = 0; j < (_methodCount - 1); j++)
            {
                stringBuilder.Append(@$"
        IContainer<MyCommandHandler{j}>,
");
            }
            stringBuilder.Append(@$"
        IContainer<MyCommandHandler{_methodCount - 1}>
    {{
        public void Dispose()
		{{
        }}
");

            for (int k = 0; k < _methodCount; k++) {
                stringBuilder.Append(@$"
        TResult IContainer<MyCommandHandler{k}>.Run<TResult, TParam>(Func<MyCommandHandler{k}, TParam, TResult> func, TParam param)
	    {{
		    MyCommandHandler{k} dependency = new MyCommandHandler{k}();

		    TResult result;
		    result = func(dependency, param);

		    return result;
	    }}
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
    using StrongInject;
    //using System;

    public static class Caller2
    {
        public static long BenchmarkMe() 
        {
            var container = new ContainerWithInterfaces();
");

            var i = 1;
            foreach (var j in _methodCallOrder)
            {
                stringBuilder.Append(@$"
            container.Run<MyCommandHandler{j}>(x => x.Handle(new MyCommand{j}{{ MyVal = {i++} }}));
");
            }

            stringBuilder.Append(@"
            //Console.WriteLine($""MyStaticClass.MyVal is {MyStaticClass.MyVal}."");
            return MyStaticClass.MyVal;
        }
    }
}
");
            return stringBuilder.ToString();
        }
    }
}
