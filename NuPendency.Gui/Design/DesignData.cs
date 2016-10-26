using DynamicData;
using NuGet;
using NuPendency.Interfaces.Model;
using System;
using System.Linq;
using System.Net.NetworkInformation;

namespace NuPendency.Gui.Design
{
    public static class DesignData
    {
        public static NuGetPackage GetNuGetPackage(int nr = 0)
        {
            string packName = string.Format("Package{0}", nr);
            return new NuGetPackage(packName, new SemanticVersion(new Version(nr, nr)));
        }

        public static ResolutionResult GetResolutionResult()
        {
            var res = new ResolutionResult("Package0", null);
            res.Packages.AddRange(Enumerable.Range(0, 5).Select(GetNuGetPackage));
            res.RootPackageId = res.Packages.Select(pack => pack.Id).First();

            var rootPack = res.Packages.First(pack => pack.Id == res.RootPackageId);
            rootPack.Dependencies.AddRange(res.Packages.Select(pack => pack.Id).Where(id => id != res.RootPackageId));
            return res;
        }
    }
}