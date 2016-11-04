using System.Collections.ObjectModel;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;
using NuGet;
using NuPendency.Interfaces.Model;

namespace NuPendency.Core.Interfaces
{
    internal interface INuGetResolutionEngine
    {
        Task<NuGetPackage> Resolve(ObservableCollection<NuGetPackage> packages,
            string packageId,
            int depth,
            CancellationToken token,
            FrameworkName targetFramework = null,
            IVersionSpec versionSpec = null);
    }
}