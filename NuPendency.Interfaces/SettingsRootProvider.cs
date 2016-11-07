using NuPendency.Commons.Interfaces;

namespace NuPendency.Interfaces
{
    public class SettingsRootProvider : ISettingsRootProvider<Settings>
    {
        private const string c_NuGetOfficialUrl = "https://packages.nuget.org/api/v2";

        public Settings CreateDefaultSettings()
        {
            var settings = new Settings
            {
                AttractionStrength = 0.3D,  //= 0.5D;
                Damping = 0.9D,             //0.9D;
                RepulsionClipping = 4000D,   //200D;
                RepulsionStrength = 2500D,//1200D;
                TimeStep = 0.95,
                MaxSearchDepth = 4
            };

            settings.Repositories.Add(c_NuGetOfficialUrl);
            return settings;
        }
    }
}