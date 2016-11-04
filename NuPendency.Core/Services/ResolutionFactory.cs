using Ninject.Parameters;
using NuPendency.Commons.Interfaces;
using NuPendency.Core.Interfaces;
using System;
using System.IO;

namespace NuPendency.Core.Services
{
    internal class ResolutionFactory : IResolutionFactory
    {
        private readonly IInstanceCreator m_InstanceCreator;

        public ResolutionFactory(IInstanceCreator instanceCreator)
        {
            m_InstanceCreator = instanceCreator;
        }

        public IResolutionEngine GetResolutionEngine(string package)
        {
            if (File.Exists(package))
            {
                var extension = Path.GetExtension(package);
                if (!string.IsNullOrEmpty(extension))
                {
                    if (extension.EndsWith(".sln", StringComparison.CurrentCultureIgnoreCase))
                    {
                        return m_InstanceCreator.CreateInstance<ISolutionResolutionEngine>(new ConstructorArgument[] { });
                    }
                    if (extension.EndsWith(".csproj", StringComparison.CurrentCultureIgnoreCase))
                    {
                        return m_InstanceCreator.CreateInstance<IProjectResolutionEngine>(new ConstructorArgument[] { });
                    }
                }
                return null;
            }
            return m_InstanceCreator.CreateInstance<INuGetResolutionEngine>(new ConstructorArgument[] { });
        }
    }
}