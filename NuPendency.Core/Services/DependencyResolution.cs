using DynamicData;
using DynamicData.Binding;
using NuGet;
using NuPendency.Core.Interfaces;
using NuPendency.Interfaces.Model;
using NuPendency.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;

namespace NuPendency.Core.Services
{
    internal class DependencyResolution : IDependencyResolution
    {
        private readonly BehaviorSubject<bool> m_Active = new BehaviorSubject<bool>(false);
        private readonly IResolutionFactory m_ResolutionFactory;
        private CancellationToken m_CancellationToken;
        private CancellationTokenSource m_CancellationTokenSource = new CancellationTokenSource();

        public DependencyResolution(IResolutionFactory resolutionFactory)
        {
            m_ResolutionFactory = resolutionFactory;
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
                result.RootPackage = rootPackageInfo;

                TryToFixMissingPackages(result);

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

        private async Task DoFindInto(ResolutionResult resultContainer, Version version = null)
        {
            using (SetActivityFlag())
            {
                m_CancellationTokenSource = new CancellationTokenSource();
                m_CancellationToken = m_CancellationTokenSource.Token;

                var rootPackageInfo = await Resolve(resultContainer.Packages, resultContainer.RootPackageName, 0, m_CancellationToken, resultContainer.TargetFramework, version.ToVersionSpec());
                if (rootPackageInfo == null)
                    return;

                resultContainer.RootPackage = rootPackageInfo;

                TryToFixMissingPackages(resultContainer);
            }
        }

        private Task<PackageBase> Resolve(ObservableCollection<PackageBase> packages,
            string packageName,
            int depth,
            CancellationToken cancellationToken,
            FrameworkName targetFramework = null,
            IVersionSpec versionSpec = null)
        {
            var resolutionEngine = m_ResolutionFactory.GetResolutionEngine(packageName);

            return resolutionEngine?.Resolve(packages, packageName, depth, cancellationToken, targetFramework,
                versionSpec);
        }

        private IDisposable SetActivityFlag()
        {
            m_Active.OnNext(true);
            return Disposable.Create(() => m_Active.OnNext(false));
        }

        private void TryToFixMissingPackages(ResolutionResult result)
        {
            foreach (var package in result.Packages)
            {
                if (!package.Dependencies.OfType<MissingNuGetPackage>().Any())
                    continue;

                foreach (var missingPackage in package.Dependencies.OfType<MissingNuGetPackage>().ToArray())
                {
                    var replacementPackage = result.Packages.SingleOrDefault(p => p.PackageId == missingPackage.PackageId);
                    if (replacementPackage == null)
                        continue;

                    package.Dependencies.Add(replacementPackage);
                    package.Dependencies.Remove(missingPackage);
                }
            }
        }
    }
}