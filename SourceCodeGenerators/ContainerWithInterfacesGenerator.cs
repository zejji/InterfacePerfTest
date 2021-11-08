using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfacePerfTest.SourceCodeGenerators
{
    internal class ContainerWithInterfacesGenerator : IContainerGenerator
    {
        private long _methodCount;

        public ContainerWithInterfacesGenerator(long methodCount)
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
            return "";
        }
    }
}
