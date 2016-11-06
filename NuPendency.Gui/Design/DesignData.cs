using NuGet;
using NuPendency.Interfaces.Model;
using System;
using System.Linq;

namespace NuPendency.Gui.Design
{
    public static class DesignData
    {
        public static NuGetPackage GetNuGetPackage(int nr = 0)
        {
            var packName = $"Package{nr}";
            return new NuGetPackage(packName, new SemanticVersion(new Version(nr, nr)), new[] { new Version(1, 2, 3, 4) });
        }

        public static ResolutionResult GetResolutionResult()
        {
            var res = new ResolutionResult("Package0", null);
            res.Packages.AddRange(Enumerable.Range(0, 5).Select(GetNuGetPackage));
            res.RootPackage = res.Packages.First();
            res.RootPackage.Dependencies.AddRange(res.Packages.Where(pack => pack != res.RootPackage));
            return res;
        }
    }
}