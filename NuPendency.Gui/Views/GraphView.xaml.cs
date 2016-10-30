using NuPendency.Gui.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NuPendency.Gui.Views
{
    /// <summary>
    /// Interaktionslogik für GraphView.xaml
    /// </summary>
    public partial class GraphView : UserControl
    {
        public GraphView()
        {
            InitializeComponent();
        }
    }

    public class MouseDownCommandParameters
    {
        public MouseButtonState ButtonState { get; set; }
        public MouseButton ChangedButton { get; set; }
        public int ClickCount { get; set; }
        public Point Position { get; set; }
        public GraphNode SenderNode { get; set; }
    }
}