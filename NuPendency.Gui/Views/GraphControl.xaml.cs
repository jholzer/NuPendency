using DynamicData.Binding;
using log4net;
using NuPendency.Commons.Extensions;
using NuPendency.Gui.ViewModels;
using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NuPendency.Gui.Views
{
    /// <summary>
    /// Interaktionslogik für GraphControl.xaml
    /// </summary>
    public partial class GraphControl : UserControl, IDisposable
    {
        public static readonly DependencyProperty AttractionStrengthProperty = DependencyProperty.Register(
            "AttractionStrength", typeof(double), typeof(GraphControl), new PropertyMetadata(default(double)));

        public static readonly DependencyProperty DampingProperty = DependencyProperty.Register(
            "Damping", typeof(double), typeof(GraphControl), new PropertyMetadata(default(double)));

        public static readonly DependencyProperty GraphEdgesProperty = DependencyProperty.Register(
            "GraphEdges", typeof(ObservableCollectionExtended<GraphEdge>), typeof(GraphControl), new PropertyMetadata(default(ObservableCollectionExtended<GraphEdge>)));

        public static readonly DependencyProperty GraphNodesProperty = DependencyProperty.Register(
                                    "GraphNodes", typeof(ObservableCollectionExtended<GraphNode>), typeof(GraphControl), new PropertyMetadata(default(ObservableCollectionExtended<GraphNode>)));

        public static readonly DependencyProperty RepulsionClippingProperty = DependencyProperty.Register(
            "RepulsionClipping", typeof(double), typeof(GraphControl), new PropertyMetadata(default(double)));

        public static readonly DependencyProperty RepulsionStrengthProperty = DependencyProperty.Register(
            "RepulsionStrength", typeof(double), typeof(GraphControl), new PropertyMetadata(default(double)));

        public static readonly DependencyProperty TimeStepProperty = DependencyProperty.Register(
            "TimeStep", typeof(double), typeof(GraphControl), new PropertyMetadata(default(double)));

        private static readonly ILog s_Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private CompositeDisposable Disposables = new CompositeDisposable();
        private bool isDragging;
        private GraphNode nodeBeingDragged;
        private Point offsetWithinNode;

        public GraphControl()
        {
            InitializeComponent();

            Observable.Interval(TimeSpan.FromMilliseconds(50))
                .ObserveOnDispatcher()
                .Subscribe(_ => CalculatePositions())
                .AddDisposableTo(Disposables);
        }

        public double AttractionStrength
        {
            get { return (double)GetValue(AttractionStrengthProperty); }
            set { SetValue(AttractionStrengthProperty, value); }
        }

        public double Damping
        {
            get { return (double)GetValue(DampingProperty); }
            set { SetValue(DampingProperty, value); }
        }

        public ObservableCollectionExtended<GraphEdge> GraphEdges
        {
            get { return (ObservableCollectionExtended<GraphEdge>)GetValue(GraphEdgesProperty); }
            set { SetValue(GraphEdgesProperty, value); }
        }

        public ObservableCollectionExtended<GraphNode> GraphNodes
        {
            get { return (ObservableCollectionExtended<GraphNode>)GetValue(GraphNodesProperty); }
            set { SetValue(GraphNodesProperty, value); }
        }

        public double RepulsionClipping
        {
            get { return (double)GetValue(RepulsionClippingProperty); }
            set { SetValue(RepulsionClippingProperty, value); }
        }

        public double RepulsionStrength
        {
            get { return (double)GetValue(RepulsionStrengthProperty); }
            set { SetValue(RepulsionStrengthProperty, value); }
        }

        public double TimeStep
        {
            get { return (double)GetValue(TimeStepProperty); }
            set { SetValue(TimeStepProperty, value); }
        }

        public void Dispose()
        {
            Disposables.Dispose();
        }

        private static double GetMovementStepSize(GraphNode node)
        {
            return Math.Sqrt(node.Velocity.X * node.Velocity.X + node.Velocity.Y * node.Velocity.Y);
        }

        private Point AttractionForce(Point node1, Point node2)
        {
            double dx = node1.X - node2.X, dy = node1.Y - node2.Y;
            var sqDist = dx * dx + dy * dy;
            var d = Math.Sqrt(sqDist);
            var mag = -AttractionStrength * 0.001 * Math.Pow(d, 1.20);

            return new Point(mag * (dx / d), mag * (dy / d));
        }

        private void CalculateEdges(GraphNode[] nodes)
        {
            foreach (var node in nodes)
            {
                foreach (var otherNode in nodes.Where(on => node.Package.Dependencies.ToArray().Contains(on.Package.Id)))
                {
                    var matchingEdge = GraphEdges.SingleOrDefault(edge => (edge.Node1 == node && edge.Node2 == otherNode));
                    if (matchingEdge == null)
                    {
                        matchingEdge = new GraphEdge(node, otherNode);
                        GraphEdges.Add(matchingEdge);
                    }

                    matchingEdge.StartPoint = node.Position;
                    matchingEdge.EndPoint = otherNode.Position;
                    matchingEdge.Selected = node.Selected || otherNode.Selected;
                }
            }
        }

        private void CalculatePositions()
        {
            var nodes = GraphNodes.ToArray();

            try
            {
                CalculateRelativePosition(nodes);
                CalculateEdges(nodes);
            }
            catch (Exception ex)
            {
                s_Logger.ErrorFormat("Error while calculating graph: {0}", ex);
            }
        }

        private void CalculateRelativePosition(GraphNode[] nodes)
        {
            foreach (var node in nodes)
            {
                if (node.LockedForMove || node.Locked)
                    continue;

                var f = new Point(0, 0); // Force
                //compute the repulsion on this node, with respect to ALL nodes
                foreach (var coulomb in nodes.Where(otherNode => node != otherNode)
                    .Select(otherNode => RepulsionForce(node.Position, otherNode.Position)))
                {
                    f.X += coulomb.X;
                    f.Y += coulomb.Y;
                }

                //compute the attraction on this node, only to the adjacent nodes
                foreach (var child in GraphEdges.Where(edge => edge.IsNodeContainedInEdge(node)).Select(edge => edge.GetOtherNode(node)))
                {
                    var hooke = AttractionForce(node.Position, child.Position);
                    f.X += hooke.X;
                    f.Y += hooke.Y;
                }

                var v = node.Velocity;

                var dampingFactor = (Damping - ((Damping * (1 - node.WeightFactor)) / 16));
                node.Velocity = new Point(
                    (v.X + TimeStep * f.X) * dampingFactor,
                    (v.Y + TimeStep * f.Y) * dampingFactor);

                if (GetMovementStepSize(node) > 1)
                {
                    var newX = (int)(node.Position.X + TimeStep * node.Velocity.X).AtLeast(0);
                    var newY = (int)(node.Position.Y + TimeStep * node.Velocity.Y).AtLeast(0);

                    node.Position = new Point(newX, newY);
                }
            }
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

            nodeBeingDragged.Position = position;
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

        private Point RepulsionForce(Point node1, Point node2)
        {
            double dx = node1.X - node2.X, dy = node1.Y - node2.Y;
            var sqDist = dx * dx + dy * dy;
            var d = Math.Sqrt(sqDist);
            var repulsion = RepulsionStrength * 1.0 / sqDist;
            repulsion += -RepulsionStrength * 0.00000006 * d;
            //clip the repulsion
            if (repulsion > RepulsionClipping) repulsion = RepulsionClipping;
            return new Point(repulsion * (dx / d), repulsion * (dy / d));
        }
    }
}