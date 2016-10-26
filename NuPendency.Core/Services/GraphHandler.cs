using System;
using NuPendency.Interfaces.Model;
using NuPendency.Interfaces.Services;
using System.Threading.Tasks;

namespace NuPendency.Core.Services
{
    internal class GraphHandler : IGraphHandler
    {
        private readonly IDependencyResolution m_DependencyResolution;

        public GraphHandler(string name, IDependencyResolution dependencyResolution)
        {
            m_DependencyResolution = dependencyResolution;
            Result = new ResolutionResult(name, null);
        }

        public string Name => Result.RootPackageName;
        public ResolutionResult Result { get; }

        public Task ResolveDependencies()
        {
            Result.Packages.Clear();
            return m_DependencyResolution.FindInto(Result, TargetVersion);
        }

        public void Cancel()
        {
            m_DependencyResolution.Cancel();
        }

        public IObservable<bool> IsActive => m_DependencyResolution.IsActive;
        public Version TargetVersion { get; set; }
    }
}