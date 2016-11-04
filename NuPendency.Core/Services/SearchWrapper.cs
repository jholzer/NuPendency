using log4net;
using NuGet;
using NuPendency.Interfaces.Model;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace NuPendency.Core.Services
{
    internal class SearchWrapper
    {
        private static readonly ILog s_Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly string m_PackageId;
        private readonly IPackageRepository m_Repo;
        private readonly TaskCompletionSource<SearchResult> m_SearchCompleted = new TaskCompletionSource<SearchResult>();

        public SearchWrapper(string packageId, IPackageRepository repo)
        {
            m_PackageId = packageId;
            m_Repo = repo;
        }

        public Task<SearchResult> Results => m_SearchCompleted.Task;

        public Task<SearchResult> Run()
        {
            Task.Run(() =>
            {
                var results = FindPackagesById(m_PackageId, m_Repo).ToArray();
                if (results.Any())
                {
                    s_Logger.InfoFormat("Found {0} results for package '{1}' in repository '{2}'", results.Length, m_PackageId, m_Repo.Source);
                    m_SearchCompleted.TrySetResult(new SearchResult(m_PackageId, m_Repo.Source, results.ToArray()));
                }
                else
                {
                    s_Logger.InfoFormat("Found no results for package '{0}' in repository '{1}'", m_PackageId, m_Repo.Source);
                }
            });

            return Results;
        }

        private static IEnumerable<IPackage> FindPackagesById(string packageId, IPackageRepository repo)
        {
            s_Logger.InfoFormat("Searching for package '{0}' in repository '{1}'", packageId, repo.Source);
            var result = repo.FindPackagesById(packageId);

            return result;
        }
    }
}