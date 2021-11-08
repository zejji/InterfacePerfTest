using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return "";
        }
    }
}
