using NuGet;
using System.IO;

namespace NuPendency.Interfaces.Model
{
    public class ProjectPackage : PackageBase
    {
        public ProjectPackage(string packageId, SemanticVersion versionInfo) : base(packageId, versionInfo, new[] { versionInfo.Version })
        {
        }

        public override string DisplayName => Path.GetFileName(PackageId);
    }
}