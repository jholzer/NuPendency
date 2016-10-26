using log4net;
using NuGet;
using NuPendency.Commons.Interfaces;
using NuPendency.Interfaces.Model;
using NuPendency.Interfaces.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Reflection;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;
using Settings = NuPendency.Interfaces.Settings;

namespace NuPendency.Core.Services
{
    public static class VersionExtensions
    {
        public static IVersionSpec ToVersionSpec(this Version version)
        {
            if (version == null)
                return null;
            return new VersionSpec(new SemanticVersion(version));
        }
    }

    internal class DependencyResolution : IDependencyResolution
    {
        private readonly BehaviorSubject<bool> m_Active = new BehaviorSubject<bool>(false);
        private readonly IRepositoryService m_RepositoryService;
        private readonly ISettingsManager<Settings> m_SettingsManager;
        private CancellationToken m_CancellationToken;
        private CancellationTokenSource m_CancellationTokenSource = new CancellationTokenSource();

        public DependencyResolution(IRepositoryService repositoryService, ISettingsManager<Settings> settingsManager)
        {
            m_RepositoryService = repositoryService;
            m_SettingsManager = settingsManager;
        }

        public IObservable<bool> IsActive => m_Active;

        public void Cancel()
        {
            m_CancellationTokenSource.Cancel();
        }

        public async Task<ResolutionResult> Find(string rootPackageName, FrameworkName targetFramework)
        {
            using (SetActivityFlag())
            {
                m_CancellationTokenSource = new CancellationTokenSource();
                m_CancellationToken = m_CancellationTokenSource.Token;

                var result = new ResolutionResult(rootPackageName, targetFramework);
                var rootPackageInfo = await Resolve(result.Packages, rootPackageName, 0, m_CancellationToken, targetFramework);
                result.RootPackageId = rootPackageInfo.Id;
                return result;
            }
        }

        public Task FindInto(ResolutionResult resultContainer)
        {
            return DoFindInto(resultContainer);
        }

        public Task FindInto(ResolutionResult resultContainer, Version version)
        {
            return DoFindInto(resultContainer, version);
        }

        private static bool TargetFrameworkMatch(FrameworkName targetFramework, PackageDependencySet set)
        {
            if (targetFramework == null)
                return true;
            return set.TargetFramework == targetFramework;
        }

        private async Task DoFindInto(ResolutionResult resultContainer, Version version = null)
        {
            using (SetActivityFlag())
            {
                m_CancellationTokenSource = new CancellationTokenSource();
                m_CancellationToken = m_CancellationTokenSource.Token;

                var rootPackageInfo = await Resolve(resultContainer.Packages, resultContainer.RootPackageName, 0, m_CancellationToken, resultContainer.TargetFramework, version.ToVersionSpec());
                if (rootPackageInfo == null)
                    return;

                resultContainer.RootPackageId = rootPackageInfo.Id;
            }
        }

        private async Task<Version[]> GetAvailableVersions(string packageId)
        {
            var packageVersions = await m_RepositoryService.FindAllVersions(packageId);
            return packageVersions.Select(pack => pack.Version.Version).ToArray();
        }

        private async Task<NuGetPackage> Resolve(ObservableCollection<NuGetPackage> packages, string packageId, int depth, CancellationToken token,
                    FrameworkName targetFramework = null, IVersionSpec versionSpec = null)
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
                    var dependingPackage = await Resolve(packages, dependency.Id, depth + 1, m_CancellationToken, targetFramework, dependency.VersionSpec) ??
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

        private IDisposable SetActivityFlag()
        {
            m_Active.OnNext(true);
            return Disposable.Create(() => m_Active.OnNext(false));
        }
    }
}