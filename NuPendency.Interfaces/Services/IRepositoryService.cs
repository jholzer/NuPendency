using DynamicData.Binding;
using NuGet;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NuPendency.Interfaces.Services
{
    public enum FindOptions
    {
        LatestRelease,
        Latest,
    }

    public interface IRepositoryService
    {
        Task<IPackage> Find(string packageId);

        Task<IPackage> Find(string packageId, Version version);

        Task<IPackage> Find(string packageId, FindOptions options);

        Task<IPackage> Find(string packageId, IVersionSpec versionSpec);

        Task<IPackage> Find(string packageId, IVersionSpec versionSpec, FindOptions options);
        Task<IEnumerable<IPackage>> FindAllVersions(string packageId);
    }
}