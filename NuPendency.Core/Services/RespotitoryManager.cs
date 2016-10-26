using NuGet;
using NuPendency.Commons.Interfaces;
using NuPendency.Interfaces.Services;
using System.Collections.ObjectModel;
using IInitializable = Ninject.IInitializable;
using Settings = NuPendency.Interfaces.Settings;

namespace NuPendency.Core.Services
{
    internal class RespotitoryManager : IRespotitoryManager, IInitializable
    {
        private readonly ISettingsManager<Settings> m_SettingsManager;

        public RespotitoryManager(ISettingsManager<Settings> settingsManager)
        {
            m_SettingsManager = settingsManager;
        }

        public ObservableCollection<IPackageRepository> Repositories { get; } =
            new ObservableCollection<IPackageRepository>();

        public void AddRepository(string path)
        {
            if (!m_SettingsManager.Settings.Repositories.Contains(path))
                m_SettingsManager.Settings.Repositories.Add(path);

            var repo = CreatePackageRepository(path);
            Repositories.Add(repo);
        }

        public void Initialize()
        {
            foreach (string repoUrl in m_SettingsManager.Settings.Repositories)
            {
                AddRepository(repoUrl);
            }
        }

        private static IPackageRepository CreatePackageRepository(string path)
        {
            return PackageRepositoryFactory.Default.CreateRepository(path);
        }
    }
}