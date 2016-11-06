using NuPendency.Interfaces.Model;
using NuPendency.Interfaces.Services;
using System;
using System.Threading.Tasks;

namespace NuPendency.Core.Services
{
    internal class GraphHandler : IGraphHandler
    {
        private readonly IDependencyResolution m_DependencyResolution;

        public GraphHandler(string packageId, IDependencyResolution dependencyResolution)
        {
            m_DependencyResolution = dependencyResolution;
            Result = new ResolutionResult(packageId, null);
        }

        public IObservable<bool> IsActive => m_DependencyResolution.IsActive;
        public ResolutionResult Result { get; }

        public Version TargetVersion { get; set; }

        public void Cancel()
        {
            m_DependencyResolution.Cancel();
        }

        public Task ResolveDependencies()
        {
            Result.Packages.Clear();
            return m_DependencyResolution.FindInto(Result, TargetVersion);
        }
    }
}