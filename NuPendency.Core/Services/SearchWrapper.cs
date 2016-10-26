using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using log4net;
using NuGet;
using NuPendency.Interfaces.Model;

namespace NuPendency.Core.Services
{
    internal class SearchWrapper
    {
        private ILog s_Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly TaskCompletionSource<SearchResult> searchCompleted = new TaskCompletionSource<SearchResult>();

        private readonly string packageId;
        private readonly IPackageRepository repo;

        public SearchWrapper(string packageId, IPackageRepository repo)
        {
            this.packageId = packageId;
            this.repo = repo;
        }

        public Task<SearchResult> Results => searchCompleted.Task;

        public Task<SearchResult> Run()
        {
            Task.Run(() =>
            {
                var results = FindPackagesById(packageId, repo);
                if (results.Any())
                {
                    s_Logger.InfoFormat("Found {0} results for package '{1}' in repository '{2}'", results.Count(), packageId, repo.Source);
                    searchCompleted.TrySetResult(new SearchResult(packageId, repo.Source, results.ToArray()));
                }
                else
                {
                    s_Logger.InfoFormat("Found no results for package '{0}' in repository '{1}'", packageId, repo.Source);
                }
            });

            return Results;
        }

        private IEnumerable<IPackage> FindPackagesById(string packageId, IPackageRepository repo)
        {
            s_Logger.InfoFormat("Searching for package '{0}' in repository '{1}'", packageId, repo.Source);
            var result = repo.FindPackagesById(packageId);
            
            return result;
        }
    }
}