using NuGet;
using NuPendency.Interfaces.Model;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;

namespace NuPendency.Core.Services.ResolutionEngines
{
    public abstract class ResolutionEngineBase
    {
        public async Task<PackageBase> Resolve(ObservableCollection<PackageBase> packages, string packageId, int depth,
            CancellationToken token, FrameworkName targetFramework = null, IVersionSpec versionSpec = null)
        {
            var result = await DoResolve(packages, packageId, depth, token, targetFramework, versionSpec);

            TryToFixMissingPackages(packages, depth + 1);

            return result;
        }

        protected abstract Task<PackageBase> DoResolve(ObservableCollection<PackageBase> packages, string packageId, int depth, CancellationToken token, FrameworkName targetFramework, IVersionSpec versionSpec);

        private static void TryToFixMissingPackages(ObservableCollection<PackageBase> packages, int targetDepth)
        {
            foreach (var package in packages)
            {
                if (!package.Dependencies.OfType<MissingNuGetPackage>().Any())
                    continue;

                foreach (var missingPackage in package.Dependencies.OfType<MissingNuGetPackage>().ToArray())
                {
                    var replacementPackage = packages.SingleOrDefault(p => p.PackageId == missingPackage.PackageId);
                    if (replacementPackage == null)
                        continue;

                    if (replacementPackage.Depth > targetDepth)
                        replacementPackage.Depth = targetDepth;

                    package.Dependencies.Add(replacementPackage);
                    package.Dependencies.Remove(missingPackage);
                }
            }
        }
    }
}