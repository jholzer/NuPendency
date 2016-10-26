using System.Threading.Tasks;

namespace NuPendency.Interfaces.Services
{
    public interface IPackageDependencyExplorerService
    {
        Task Explore(string packageId, string version);
    }
}