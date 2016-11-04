namespace NuPendency.Core.Interfaces
{
    internal interface IResolutionFactory
    {
        INuGetResolutionEngine GetResolutionEngine(string package);
    }
}