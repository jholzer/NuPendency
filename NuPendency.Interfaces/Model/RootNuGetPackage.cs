using System;
using NuGet;

namespace NuPendency.Interfaces.Model
{
    public class RootNuGetPackage : NuGetPackage
    {
        public RootNuGetPackage(string packageId, SemanticVersion versionInfo, Version[] availableVersions) : base(packageId, versionInfo)
        {
            AvailableVersions = availableVersions;
        }

        public Version[] AvailableVersions { get; }
    }
}