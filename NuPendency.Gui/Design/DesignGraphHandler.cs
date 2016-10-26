using NuPendency.Interfaces.Model;
using NuPendency.Interfaces.Services;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace NuPendency.Gui.Design
{
    public class DesignGraphHandler : IGraphHandler
    {
        public ResolutionResult Result { get; } = DesignData.GetResolutionResult();

        public Task ResolveDependencies()
        {
            throw new NotImplementedException();
        }

        public Task ResolveDependencies(Version version)
        {
            throw new NotImplementedException();
        }

        public void Cancel()
        {
            throw new NotImplementedException();
        }

        public IObservable<bool> IsActive => Observable.Return(false);
        public Version TargetVersion { get; set; }
    }
}