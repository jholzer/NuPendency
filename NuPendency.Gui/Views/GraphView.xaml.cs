using NuPendency.Gui.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NuPendency.Gui.Views
{
    /// <summary>
    /// Interaktionslogik für GraphView.xaml
    /// </summary>
    public partial class GraphView : UserControl
    {
        private bool isDragging;
        private GraphNode nodeBeingDragged;
        private Point offsetWithinNode;

        public GraphView()
        {
            InitializeComponent();
        }

        private void DeSelectAllNodes(GraphViewModel graphViewModel)
        {
            foreach (var node in graphViewModel.GraphNodes)
                node.Selected = false;
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            nodeBeingDragged = null;
            isDragging = false;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!isDragging)
                return;

            var frameworkElement = sender as FrameworkElement;
            if (frameworkElement == null)
                return;

            var position = e.GetPosition(frameworkElement);
            position.X += (-offsetWithinNode.X);
            position.Y += (-offsetWithinNode.Y);

            nodeBeingDragged.Position = position;//new Point(nodeBeingDragged.Position.X + position.X, nodeBeingDragged.Position.Y + position.Y);
        }

        private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            DeSelectAllNodes(DataContext as GraphViewModel);

            var frameworkElement = sender as Border;
            if (frameworkElement == null)
                return;

            nodeBeingDragged = frameworkElement.DataContext as GraphNode;
            if (nodeBeingDragged == null)
                return;

            nodeBeingDragged.Selected = true;

            if (e.ClickCount == 2)
            {
                nodeBeingDragged.Locked = !nodeBeingDragged.Locked;
            }
            else if (e.ClickCount == 1)
            {
                nodeBeingDragged.Locked = true;
                nodeBeingDragged.LockedForMove = true;
                offsetWithinNode = e.GetPosition(frameworkElement);
                isDragging = true;
            }
        }

        private void OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (nodeBeingDragged == null)
                return;
            nodeBeingDragged.LockedForMove = false;
            isDragging = false;
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