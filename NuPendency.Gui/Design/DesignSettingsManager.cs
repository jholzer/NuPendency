using NuPendency.Commons.Interfaces;
using NuPendency.Interfaces;

namespace NuPendency.Gui.Design
{
    public class DesignSettingsManager : ISettingsManager<Settings>
    {
        public DesignSettingsManager()
        {
            Settings = new Settings();
            Settings.Repositories.Add("https://Repo1.org");
            Settings.Repositories.Add("https://Repo2.org");
            Settings.Repositories.Add("https://Repo3.org");
        }

        public Settings Settings { get; }
    }
}