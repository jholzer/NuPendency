using NuGet;
using System;
using System.Collections.ObjectModel;

namespace NuPendency.Interfaces.Model
{
    public abstract class PackageBase
    {
        protected PackageBase(string packageId, SemanticVersion versionInfo, Version[] availableVersions)
        {
            PackageId = packageId;
            VersionInfo = versionInfo;
            AvailableVersions = availableVersions;
        }

        public Version[] AvailableVersions { get; }
        public ObservableCollection<Guid> Dependencies { get; } = new ObservableCollection<Guid>();
        public int Depth { get; set; }
        public abstract string DisplayName { get; }
        public Guid Id { get; } = Guid.NewGuid();
        public string PackageId { get; }
        public SemanticVersion VersionInfo { get; }
    }
}