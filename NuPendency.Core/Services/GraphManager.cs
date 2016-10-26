using Ninject.Parameters;
using NuPendency.Commons.Interfaces;
using NuPendency.Interfaces.Services;
using System.Collections.ObjectModel;

namespace NuPendency.Core.Services
{
    internal class GraphManager : IGraphManager
    {
        private readonly IInstanceCreator m_InstanceCreator;

        public GraphManager(IInstanceCreator instanceCreator)
        {
            m_InstanceCreator = instanceCreator;
        }

        public ObservableCollection<IGraphHandler> Documents { get; } = new ObservableCollection<IGraphHandler>();

        public IGraphHandler CreateNewDocument(string name)
        {
            var graphHandler = m_InstanceCreator.CreateInstance<IGraphHandler>(new[] { new ConstructorArgument("name", name) });
            Documents.Add(graphHandler);

            return graphHandler;
        }

        public void Delete(IGraphHandler doc)
        {
            Documents.Remove(doc);
        }
    }
}