using NuGet;
using NuPendency.Commons.Interfaces;
using NuPendency.Core.Interfaces;
using NuPendency.Interfaces.Model;
using NuPendency.Interfaces.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;
using Settings = NuPendency.Interfaces.Settings;

namespace NuPendency.Core.Services
{
    internal class NuGetResolutionEngine : INuGetResolutionEngine
    {
        private readonly IRepositoryService m_RepositoryService;
        private readonly ISettingsManager<Settings> m_SettingsManager;

        public NuGetResolutionEngine(IRepositoryService repositoryService, ISettingsManager<Settings> settingsManager)
        {
            m_RepositoryService = repositoryService;
            m_SettingsManager = settingsManager;
        }

        public async Task<NuGetPackage> Resolve(ObservableCollection<NuGetPackage> packages,
            string packageId,
            int depth,
            CancellationToken token,
            FrameworkName targetFramework = null,
            IVersionSpec versionSpec = null)
        {
            if (depth > m_SettingsManager.Settings.MaxSearchDepth)
                return null;

            var packageInfo = await m_RepositoryService.Find(packageId, versionSpec);
            if (packageInfo == null)
                return null;

            var package = packages.FirstOrDefault(pack => pack.PackageId == packageInfo.Id && pack.VersionInfo == packageInfo.Version);
            if (package != null)
                return package;

            package = depth == 0 ? new RootNuGetPackage(packageInfo.Id, packageInfo.Version, await GetAvailableVersions(packageId)) : new NuGetPackage(packageInfo.Id, packageInfo.Version);
            package.Depth = depth;
            packages.Add(package);

            if (token.IsCancellationRequested)
                return null;

            foreach (var dependencySet in packageInfo.DependencySets.Where(set => TargetFrameworkMatch(targetFramework, set)))
            {
                foreach (var dependency in dependencySet.Dependencies)
                {
                    var dependingPackage = await Resolve(packages, dependency.Id, depth + 1, token, targetFramework, dependency.VersionSpec) ??
                                           new MissingNuGetPackage(dependency.Id);
                    package.Dependencies.Add(dependingPackage.Id);

                    if (token.IsCancellationRequested)
                        break;
                }

                if (token.IsCancellationRequested)
                    break;
            }
            return package;
        }

        private static bool TargetFrameworkMatch(FrameworkName targetFramework, PackageDependencySet set)
        {
            if (targetFramework == null)
                return true;
            return set.TargetFramework == targetFramework;
        }

        private async Task<Version[]> GetAvailableVersions(string packageId)
        {
            var packageVersions = await m_RepositoryService.FindAllVersions(packageId);
            return packageVersions.Select(pack => pack.Version.Version).ToArray();
        }
    }
}