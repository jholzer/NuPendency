using NuPendency.Commons.Model;
using NuPendency.Interfaces.Model;
using System.Windows;

namespace NuPendency.Gui.ViewModels
{
    public class GraphNode : BaseObject
    {
        private bool m_Locked;
        private Point m_Position;
        private bool m_Selected;

        public bool Locked
        {
            get { return m_Locked; }
            set
            {
                if (m_Locked == value) return;
                m_Locked = value;
                RaisePropertyChanged();
            }
        }

        public bool LockedForMove { get; set; }
        public NuGetPackage Package { get; set; }

        public Point Position
        {
            get { return m_Position; }
            set
            {
                if (m_Position == value) return;
                m_Position = value;
                RaisePropertyChanged();
            }
        }

        public int ReferencedByCount { get; set; }

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

        public Point Velocity { get; set; }
        public double WeightFactor { get; set; }

        public override string ToString()
        {
            return $"{Package.PackageId}/{Package.VersionInfo}, Pos: X{Position.X:F1}/Y{Position.Y:F1}, Weight:{WeightFactor:F1}, Lock:{LockedForMove}, VelX: {Velocity.X:F1}/VelY: {Velocity.Y:F1}";
        }
    }
}