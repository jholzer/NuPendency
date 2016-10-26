namespace NuPendency.Commons.Interfaces
{
    public interface ISettingsRootProvider<T>
    {
        T CreateDefaultSettings();
    }
}