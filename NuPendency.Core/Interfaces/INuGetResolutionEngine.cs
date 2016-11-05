using NuGet;
using NuPendency.Interfaces.Model;
using System.Collections.ObjectModel;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;

namespace NuPendency.Core.Interfaces
{
    internal interface INuGetResolutionEngine : IResolutionEngine
    {
    }

    internal interface IProjectResolutionEngine : IResolutionEngine
    {
    }

    internal interface IResolutionEngine
    {
        Task<PackageBase> Resolve(ObservableCollection<PackageBase> packages,
            string packageId,
            int depth,
            CancellationToken token,
            FrameworkName targetFramework = null,
            IVersionSpec versionSpec = null);
    }

    internal interface ISolutionResolutionEngine : IResolutionEngine
    {
    }
}