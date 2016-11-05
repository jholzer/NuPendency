using System;

namespace NuPendency.Interfaces.Model
{
    public class MissingNuGetPackage : PackageBase
    {
        public MissingNuGetPackage(string packageId) : base(packageId, null, new Version[] { })
        {
        }

        public override string DisplayName => $"{PackageId} ???";
    }
}