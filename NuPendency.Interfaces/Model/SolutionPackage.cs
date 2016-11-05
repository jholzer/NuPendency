using NuGet;
using System;
using System.IO;

namespace NuPendency.Interfaces.Model
{
    public class SolutionPackage : PackageBase
    {
        public SolutionPackage(string packageId, SemanticVersion versionInfo) : base(packageId, versionInfo, new[] { versionInfo.Version })
        {
        }

        public override string DisplayName => Path.GetFileName(PackageId);
    }
}