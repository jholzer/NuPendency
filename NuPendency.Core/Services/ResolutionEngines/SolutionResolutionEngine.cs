using CWDev.SLNTools.Core;
using NuGet;
using NuPendency.Core.Interfaces;
using NuPendency.Interfaces.Model;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;

namespace NuPendency.Core.Services.ResolutionEngines
{
    internal class SolutionResolutionEngine : ResolutionEngineBase, ISolutionResolutionEngine
    {
        private readonly IResolutionFactory m_ResolutionFactory;

        public SolutionResolutionEngine(IResolutionFactory resolutionFactory)
        {
            m_ResolutionFactory = resolutionFactory;
        }

        protected override async Task<PackageBase> DoResolve(ObservableCollection<PackageBase> packages, string packageId, int depth, CancellationToken token, FrameworkName targetFramework = null, IVersionSpec versionSpec = null)
        {
            var result = packages.FirstOrDefault(pack => pack.PackageId == packageId);
            if (result != null)
                return result;

            result = new SolutionPackage(packageId, new SemanticVersion(new Version())) { Depth = depth };
            packages.Add(result);

            if (token.IsCancellationRequested)
                return null;

            using (var solutionFileReader = new SolutionFileReader(packageId))
            {
                var readSolutionFile = solutionFileReader.ReadSolutionFile();

                var basePath = Path.GetDirectoryName(packageId);
                if (string.IsNullOrEmpty(basePath))
                    return null;

                foreach (var project in readSolutionFile.Projects)
                {
                    var projectPath = Path.Combine(basePath, project.RelativePath);
                    var resolutionEngine = m_ResolutionFactory.GetResolutionEngine(projectPath);
                    if (resolutionEngine == null)
                        continue;

                    var dependingPackage = await resolutionEngine.Resolve(packages, projectPath, depth + 1, token, targetFramework, versionSpec);
                    result.Dependencies.Add(dependingPackage);

                    if (token.IsCancellationRequested)
                        break;
                }
            }
            return result;
        }
    }
}