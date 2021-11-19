using System.Text;

namespace InterfacePerfTest.SourceCodeBuilders
{
    internal class ContainerNoInterfacesSourceCodeBuilder : IContainerSourceCodeBuilder
    {
        private int _methodCount;
        private IEnumerable<int> _methodCallOrder;

        public ContainerNoInterfacesSourceCodeBuilder(int methodCount, IEnumerable<int> methodCallOrder)
        {
            _methodCount = methodCount;
            _methodCallOrder = methodCallOrder;
        }

        public string GetContainerSourceText()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(@"
namespace MyCode
{
    using System;

    public static class MyStaticClass1
    {
        public static int MyVal { get; set; } = 0;
    }
");
            for (int i = 0; i < _methodCount; i++)
            {
                stringBuilder.Append(@$"
    public readonly ref struct MyCommand1_{i}
    {{
        public int MyVal {{ get; init; }}
	
	    public MyCommand1_{i}(int myVal)
	    {{
            MyVal = myVal;
	    }}
    }}

    public class MyCommandHandler1_{i}
    {{
        public void Handle(in MyCommand1_{i} command)
        {{
            MyStaticClass1.MyVal += (command.MyVal + {i});
        }}
    }}
");
            }

            stringBuilder.Append(@"
    public class ContainerNoInterfaces
    {
        public void Dispose()
		{
        }
");

            for (int k = 0; k < _methodCount; k++)
            {
                stringBuilder.Append(@$"
        public void Run(Action<MyCommandHandler1_{k}> func)
	    {{
		    MyCommandHandler1_{k} dependency = new MyCommandHandler1_{k}();

		    func(dependency);
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
    //using System;

    public static class NoInterfaceContainerCaller
    {
        public static long BenchmarkMe() 
        {
            var container = new ContainerNoInterfaces();
");

            var i = 1;
            foreach (var j in _methodCallOrder)
            {
                stringBuilder.Append(@$"
            container.Run((MyCommandHandler1_{j} x) => x.Handle(new MyCommand1_{j}({i++})));
");
            }

            stringBuilder.Append(@"
            //Console.WriteLine($""MyStaticClass1.MyVal is {MyStaticClass1.MyVal}."");
            return MyStaticClass1.MyVal;
        }
    }
}
");
            return stringBuilder.ToString();
        }
    }
}
