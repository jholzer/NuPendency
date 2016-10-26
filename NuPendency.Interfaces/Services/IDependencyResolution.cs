using System;
using NuPendency.Interfaces.Model;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using NuGet;

namespace NuPendency.Interfaces.Services
{
    public interface IDependencyResolution
    {
        Task<ResolutionResult> Find(string rootPackageName, FrameworkName targetFramework);
        Task FindInto(ResolutionResult resultContainer, Version version);
        Task FindInto(ResolutionResult resultContainer);
        void Cancel();
        IObservable<bool> IsActive { get; }
    }
}