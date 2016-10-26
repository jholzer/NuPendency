using log4net;
using NuGet;
using NuPendency.Commons.Interfaces;
using NuPendency.Interfaces.Model;
using NuPendency.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Reflection;
using System.Threading.Tasks;
using Settings = NuPendency.Interfaces.Settings;

namespace NuPendency.Core.Services
{
    internal class RepositoryService : IRepositoryService
    {
        private static readonly ILog s_Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IRespotitoryManager m_RespotitoryManager;
        private readonly ISettingsManager<Settings> m_SettingsManager;
        private TimeSpan m_Timeout = TimeSpan.FromSeconds(15);

        public RepositoryService(IRespotitoryManager respotitoryManager, ISettingsManager<Settings> settingsManager)
        {
            m_RespotitoryManager = respotitoryManager;
            m_SettingsManager = settingsManager;
        }

        public async Task<IPackage> Find(string packageId, Version version)
        {
            var foundPackages = await FindAllVersions(packageId);

            var matchingPackage = foundPackages.FirstOrDefault(package => package?.Version?.Version == version);
            return matchingPackage;
        }

        public Task<IPackage> Find(string packageId)
        {
            return Find(packageId, FindOptions.LatestRelease);
        }

        public Task<IPackage> Find(string packageId, IVersionSpec versionSpec)
        {
            return Find(packageId, versionSpec, FindOptions.LatestRelease);
        }

        public async Task<IPackage> Find(string packageId, IVersionSpec versionSpec, FindOptions options)
        {
            var foundPackages = await FindAllVersions(packageId);
            var versionMatches = foundPackages.Where(pack => IsMinVersionMatch(pack, versionSpec) && IsMaxVersionMatch(pack, versionSpec)).ToArray();
            return GetVersionByFindOption(versionMatches, options);
        }

        public Task<IPackage> Find(string packageId, FindOptions options)
        {
            return Find(packageId, null, FindOptions.LatestRelease);
        }

        public async Task<IEnumerable<IPackage>> FindAllVersions(string packageId)
        {
            if (IsPackageExcluded(packageId))
                return new IPackage[] { };

            var timeoutTask = Observable.Timer(m_Timeout)
                .Select(l => new SearchResult(packageId, "all", new IPackage[] { }) { Timeout = true })
                .ToTask();

            var searches = m_RespotitoryManager.Repositories
                .Select(repo => new SearchWrapper(packageId, repo))
                .ToArray();

            if (!searches.Any())
                return new IPackage[] { };

            var searchTasks = searches.Select(wrapper => wrapper.Run());

            var stopTasks = new List<Task<SearchResult>> { timeoutTask };
            stopTasks.AddRange(searchTasks);

            var firstSuccessFullSearchResult = await Task.WhenAny(stopTasks);
            var foundPackages = firstSuccessFullSearchResult.Result;

            if (foundPackages.Timeout)
                s_Logger.WarnFormat("No packages found for package {0} after {1} seconds", packageId, m_Timeout.TotalSeconds);

            return foundPackages.FoundPackages;
        }

        private static IPackage GetVersionByFindOption(IEnumerable<IPackage> foundPackages, FindOptions options)
        {
            IPackage result = null;
            switch (options)
            {
                case FindOptions.Latest:
                    result = foundPackages.OrderByDescending(package => package.Version.Version).FirstOrDefault();
                    break;

                case FindOptions.LatestRelease:
                    result = foundPackages.Where(package => string.IsNullOrEmpty(package.Version.SpecialVersion))
                        .OrderByDescending(package => package.Version.Version).FirstOrDefault();
                    break;
            }
            return result;
        }

        private static bool IsMaxVersionMatch(IPackage pack, IVersionSpec versionSpec)
        {
            if (versionSpec?.MaxVersion == null)
                return true;

            if (versionSpec.IsMaxInclusive)
                return pack.Version <= versionSpec.MaxVersion;
            return pack.Version <= versionSpec.MaxVersion;
        }

        private static bool IsMinVersionMatch(IPackage pack, IVersionSpec versionSpec)
        {
            if (versionSpec?.MinVersion == null)
                return true;

            if (versionSpec.IsMinInclusive)
                return versionSpec.MinVersion <= pack.Version;
            return versionSpec.MinVersion < pack.Version;
        }

        private static bool IsPackageExcluded(string packageId, string exclude)
        {
            packageId = packageId.ToLowerInvariant();
            exclude = exclude.ToLowerInvariant();
            string trimmedExclude = exclude.Trim('*');
            if (exclude.StartsWith("*") && exclude.EndsWith("*"))
            {
                return packageId.Contains(trimmedExclude);
            }
            if (exclude.StartsWith("*"))
            {
                return packageId.EndsWith(trimmedExclude);
            }
            if (exclude.EndsWith("*"))
            {
                return packageId.StartsWith(trimmedExclude);
            }
            return packageId == exclude;
        }

        private bool IsPackageExcluded(string packageId)
        {
            return m_SettingsManager.Settings.ExcludedPackages.Any(exclude => IsPackageExcluded(packageId, exclude));
        }
    }
}