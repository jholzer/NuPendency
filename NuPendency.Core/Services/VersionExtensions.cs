using System;
using NuGet;

namespace NuPendency.Core.Services
{
    public static class VersionExtensions
    {
        public static IVersionSpec ToVersionSpec(this Version version)
        {
            if (version == null)
                return null;
            return new VersionSpec(new SemanticVersion(version));
        }
    }
}