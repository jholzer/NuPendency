using System;
using System.Collections.Generic;
using System.Linq;
using NuGet;

namespace NuPendency.Interfaces.Model
{
    public class SearchResult
    {
        public SearchResult(string searchPackageId, string repositoryUrl, IPackage[] foundPackages)
        {
            SearchPackageId = searchPackageId;
            RepositoryUrl = repositoryUrl;
            FoundPackages = foundPackages;
        }
        public string SearchPackageId { get; private set; }
        public string RepositoryUrl { get; private set; }
        public IEnumerable<IPackage> FoundPackages { get; private set; }
        public bool Timeout { get; set; }
    }
}
