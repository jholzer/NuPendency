namespace NuPendency.Commons.Interfaces
{
    public interface ISettingsManager<T>
    {
        T Settings { get; }
    }
}