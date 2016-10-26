using NuPendency.Commons.Model;
using System.Windows;

namespace NuPendency.Gui.ViewModels
{
    public class GraphEdge : BaseObject
    {
        private Point m_EndPoint;
        private bool m_Selected;
        private Point m_StartPoint;

        public GraphEdge(GraphNode node1, GraphNode node2)
        {
            Node1 = node1;
            Node2 = node2;
        }

        public Point EndPoint
        {
            get { return m_EndPoint; }
            set
            {
                if (m_EndPoint == value) return;
                m_EndPoint = value;
                RaisePropertyChanged();
            }
        }

        public GraphNode Node1 { get; }
        public GraphNode Node2 { get; }

        public bool Selected
        {
            get { return m_Selected; }
            set
            {
                if (m_Selected == value) return;
                m_Selected = value;
                RaisePropertyChanged();
            }
        }

        public Point StartPoint
        {
            get { return m_StartPoint; }
            set
            {
                if (m_StartPoint == value) return;
                m_StartPoint = value;
                RaisePropertyChanged();
            }
        }
    }
}