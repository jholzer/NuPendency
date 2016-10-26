using NuGet;
using System;
using System.Linq;

namespace NuPendency.Core.Services
{
    internal class LocalRepositoryHandler : IPackageRepository
    {
        public PackageSaveModes PackageSaveMode { get; set; } = PackageSaveModes.Nupkg;

        public string Source { get; } = string.Empty;

        public bool SupportsPrereleasePackages { get; } = false;

        public void AddPackage(IPackage package)
        {
            throw new NotImplementedException();
        }

        public IQueryable<IPackage> GetPackages()
        {
            throw new NotImplementedException();
        }

        public void RemovePackage(IPackage package)
        {
            throw new NotImplementedException();
        }
    }
}