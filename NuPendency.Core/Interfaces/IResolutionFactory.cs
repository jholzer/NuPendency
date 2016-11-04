namespace NuPendency.Core.Interfaces
{
    internal interface IResolutionFactory
    {
        IResolutionEngine GetResolutionEngine(string package);
    }
}