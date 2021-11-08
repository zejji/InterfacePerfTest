namespace InterfacePerfTest.SourceCodeGenerators
{
    internal interface IContainerGenerator
    {
        string GetCallingCodeSourceText();
        string GetContainerSourceText();
    }
}