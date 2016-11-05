using NuGet;
using System;

namespace NuPendency.Interfaces.Model
{
    public class NuGetPackage : PackageBase
    {
        public NuGetPackage(string packageId, SemanticVersion versionInfo, Version[] availableVersions) : base(packageId, versionInfo, availableVersions)
        {
        }

        public override string DisplayName => PackageId;

        public override string ToString()
        {
            return $"{PackageId} {VersionInfo} ({Dependencies.Count} dependencies)";
        }
    }
}