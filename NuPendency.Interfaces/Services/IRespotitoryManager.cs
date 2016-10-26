using NuGet;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NuPendency.Interfaces.Services
{
    public interface IRespotitoryManager
    {
        ObservableCollection<IPackageRepository> Repositories { get; }

        void AddRepository(string path);
    }
}