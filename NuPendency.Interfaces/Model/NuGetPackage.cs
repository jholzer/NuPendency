using NuGet;
using System;
using System.Collections.ObjectModel;

namespace NuPendency.Interfaces.Model
{
    public class NuGetPackage
    {
        public NuGetPackage(string packageId, SemanticVersion versionInfo)
        {
            PackageId = packageId;
            VersionInfo = versionInfo;
        }

        public ObservableCollection<Guid> Dependencies { get; } = new ObservableCollection<Guid>();
        public int Depth { get; set; }
        public Guid Id { get; } = Guid.NewGuid();
        public string PackageId { get; }
        public SemanticVersion VersionInfo { get; }

        public override string ToString()
        {
            return $"{PackageId} {VersionInfo} ({Dependencies.Count} dependencies)";
        }
    }
}