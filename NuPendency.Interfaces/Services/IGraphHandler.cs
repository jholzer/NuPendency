using System;
using NuPendency.Interfaces.Model;
using System.Threading.Tasks;

namespace NuPendency.Interfaces.Services
{
    public interface IGraphHandler
    {
        ResolutionResult Result { get; }
        Task ResolveDependencies();
        void Cancel();
        IObservable<bool> IsActive { get; }
        Version TargetVersion { get; set; }
    }
}