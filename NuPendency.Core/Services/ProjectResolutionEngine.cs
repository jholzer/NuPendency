using NuGet;
using NuPendency.Core.Interfaces;
using NuPendency.Interfaces.Model;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NuPendency.Core.Services
{
    internal class ProjectResolutionEngine : IProjectResolutionEngine
    {
        private readonly IResolutionFactory m_ResolutionFactory;

        public ProjectResolutionEngine(IResolutionFactory resolutionFactory)
        {
            m_ResolutionFactory = resolutionFactory;
        }

        public async Task<PackageBase> Resolve(ObservableCollection<PackageBase> packages, string packageId, int depth, CancellationToken token, FrameworkName targetFramework = null, IVersionSpec versionSpec = null)
        {
            var result = new ProjectPackage(packageId, new SemanticVersion(new Version())) { Depth = depth };
            packages.Add(result);

            var baseDir = Path.GetDirectoryName(packageId);
            if (string.IsNullOrEmpty(baseDir))
                return null;

            var packageFile = Path.Combine(baseDir, "packages.config");

            if (!File.Exists(packageFile))
                return null;

            if (token.IsCancellationRequested)
                return null;

            using (TextReader reader = new StreamReader(packageFile))
            {
                var xDocument = XDocument.Load(reader);
                var packagesNode = xDocument.Element("packages");

                if (packagesNode == null)
                    return null;

                var packageNodes = packagesNode.Elements("package");

                foreach (var pack in packageNodes)
                {
                    var id = pack.Attribute("id");
                    if (id == null)
                        continue;
                    var version = pack.Attribute("version");
                    if (version == null)
                        continue;

                    var resolutionEngine = m_ResolutionFactory.GetResolutionEngine(id.Value);
                    if (resolutionEngine == null)
                        continue;

                    var dependingPackage = await resolutionEngine.Resolve(packages, id.Value, depth + 1, token, null,
                        new VersionSpec(new SemanticVersion(version.Value)));

                    result.Dependencies.Add(dependingPackage);

                    if (token.IsCancellationRequested)
                        break;
                }
            }

            return result;
        }
    }
}