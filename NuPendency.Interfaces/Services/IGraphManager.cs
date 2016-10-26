using NuPendency.Interfaces.Model;
using System.Collections.ObjectModel;

namespace NuPendency.Interfaces.Services
{
    public interface IGraphManager
    {
        ObservableCollection<IGraphHandler> Documents { get; }

        IGraphHandler CreateNewDocument(string name);

        void Delete(IGraphHandler doc);
    }
}