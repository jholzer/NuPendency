using Ninject.Modules;
using NuPendency.Commons.Interfaces;
using NuPendency.Commons.Model;
using NuPendency.Core.Interfaces;
using NuPendency.Core.Services;
using NuPendency.Core.Services.ResolutionEngines;
using NuPendency.Interfaces;
using NuPendency.Interfaces.Services;

namespace NuPendency.Core
{
    public class CoreModuleCatalog : NinjectModule
    {
        public override void Load()
        {
            Bind<Settings>().ToSelf().InSingletonScope();
            Bind<IRespotitoryManager>().To<RespotitoryManager>().InSingletonScope();

            Bind<IDependencyResolution>().To<DependencyResolution>();
            Bind<IRepositoryService>().To<RepositoryService>();
            Bind<IGraphHandler>().To<GraphHandler>();
            Bind<IGraphManager>().To<GraphManager>().InSingletonScope();

            Bind<ISettingsRootProvider<Settings>>().To<SettingsRootProvider>().InSingletonScope();
            Bind<ISettingsManager<Settings>>().To<SettingsManager<Settings>>().InSingletonScope();

            #region internal

            Bind<IResolutionFactory>().To<ResolutionFactory>();
            Bind<INuGetResolutionEngine>().To<NuGetResolutionEngine>();
            Bind<ISolutionResolutionEngine>().To<SolutionResolutionEngine>();
            Bind<IProjectResolutionEngine>().To<ProjectResolutionEngine>();

            #endregion internal
        }
    }
}