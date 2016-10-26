using DynamicData;
using DynamicData.Binding;
using log4net;
using NuPendency.Commons.Extensions;
using NuPendency.Commons.Interfaces;
using NuPendency.Gui.Design;
using NuPendency.Gui.Views;
using NuPendency.Interfaces;
using NuPendency.Interfaces.Model;
using NuPendency.Interfaces.Services;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace NuPendency.Gui.ViewModels
{
    public class DesignGraphViewModel : GraphViewModel
    {
        [SuppressMessage("ReSharper", "DoNotCallOverridableMethodsInConstructor")]
        public DesignGraphViewModel() : base(new DesignGraphHandler(), new DesignSettingsManager())
        {
            Init();
        }
    }

    public class GraphViewModel : ViewModelBase
    {
        private static readonly ILog s_Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly Random m_Random = new Random();
        private readonly ISettingsManager<Settings> m_SettingsManager;
        private IEnumerable<Version> m_AllFoundRootVersions;
        private ObservableAsPropertyHelper<bool> m_IsActiveHelper;
        private ObservableAsPropertyHelper<int> m_NodeCountHelper;
        private Version m_SelectedVersion;

        public GraphViewModel(IGraphHandler model, ISettingsManager<Settings> settingsManager)
        {
            m_SettingsManager = settingsManager;
            GraphHandler = model;
        }

        public IEnumerable<Version> AllFoundRootVersions
        {
            get { return m_AllFoundRootVersions; }
            private set
            {
                m_AllFoundRootVersions = value;
                raisePropertyChanged();
            }
        }

        public ReactiveCommand<Unit> CmdPreviewMouseDown { get; set; }
        public ReactiveCommand<Unit> CmdPreviewMouseUp { get; set; }

        public ObservableCollectionExtended<GraphEdge> GraphEdges { get; } = new ObservableCollectionExtended<GraphEdge>();
        public IGraphHandler GraphHandler { get; }
        public ObservableCollectionExtended<GraphNode> GraphNodes { get; } = new ObservableCollectionExtended<GraphNode>();
        public bool IsActive => m_IsActiveHelper.Value;
        public string Name => ResolutionResult.RootPackageName;
        public int NodeCount => m_NodeCountHelper.Value;
        public ResolutionResult ResolutionResult => GraphHandler.Result;

        public Version SelectedVersion
        {
            get { return m_SelectedVersion; }
            set
            {
                if (m_SelectedVersion == value) return;
                m_SelectedVersion = value;
                raisePropertyChanged();
            }
        }

        public GraphNode CreateDataNode(NuGetPackage pack)
        {
            return new GraphNode
            {
                Position = new Point(pack.Depth * 50 + 500 * m_Random.NextDouble(), pack.Depth * 100 + 20 * m_Random.NextDouble()),
                Package = pack
            };
        }

        public override void Init()
        {
            ResolutionResult.Packages
                .ToObservableChangeSet()
                .Filter(pack => pack is RootNuGetPackage)
                .ObserveOnDispatcher()
                .Subscribe(set => FillVersions())
                .AddDisposableTo(Disposables);

            this.WhenAnyValue(vm => vm.SelectedVersion)
                .Subscribe(version =>
                           {
                               if (version != null) GraphHandler.TargetVersion = version;
                           })
                .AddDisposableTo(Disposables);

            m_IsActiveHelper = GraphHandler.IsActive
                .ToProperty(this, vm => vm.IsActive)
                .AddDisposableTo(Disposables);

            m_NodeCountHelper = ResolutionResult.Packages.ToObservableChangeSet()
                .Select(set => ResolutionResult.Packages.Count)
                .ObserveOnDispatcher()
                .ToProperty(this, vm => vm.NodeCount)
                .AddDisposableTo(Disposables);

            ResolutionResult.Packages.ToObservableChangeSet()
                .Transform(CreateDataNode)
                .DisposeMany()
                .ObserveOnDispatcher()
                .Bind(GraphNodes)
                .Do(_ => TryUpdateNodeLevel())
                .Subscribe()
                .AddDisposableTo(Disposables);

            Observable.Interval(TimeSpan.FromMilliseconds(50))
                .ObserveOnDispatcher()
                .Subscribe(_ => CalculatePositions())
                .AddDisposableTo(Disposables);

            CmdPreviewMouseDown = ReactiveCommand.CreateAsyncTask(o =>
            {
                MouseDownCommandParameters param = o as MouseDownCommandParameters;
                if (param != null)
                {
                    if (param.ChangedButton == MouseButton.Left)
                    {
                        param.SenderNode.Position = param.Position;
                        param.SenderNode.LockedForMove = true;
                    }
                    else if (param.ChangedButton == MouseButton.Right)
                    {
                        param.SenderNode.LockedForMove = false;
                    }
                }
                return Task.FromResult(Unit.Default);
            });

            CmdPreviewMouseUp = ReactiveCommand.CreateAsyncTask(o =>
            {
                Console.WriteLine(o);
                return Task.FromResult(Unit.Default);
            });
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
            var mag = -m_SettingsManager.Settings.AttractionStrength * 0.001 * Math.Pow(d, 1.20);

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

            RemoveOrphanedEdges();
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
                foreach (var child in nodes.Where(conNode => node.Package.Dependencies.ToArray().Contains(conNode.Package.Id)))
                {
                    var hooke = AttractionForce(node.Position, child.Position);
                    f.X += hooke.X;
                    f.Y += hooke.Y;
                }

                var v = node.Velocity;

                var dampingFactor = (m_SettingsManager.Settings.Damping - ((m_SettingsManager.Settings.Damping * (1 - node.WeightFactor)) / 16));
                node.Velocity = new Point(
                    (v.X + m_SettingsManager.Settings.TimeStep * f.X) * dampingFactor,
                    (v.Y + m_SettingsManager.Settings.TimeStep * f.Y) * dampingFactor);

                if (GetMovementStepSize(node) > 1)
                {
                    var newX = (int)(node.Position.X + m_SettingsManager.Settings.TimeStep * node.Velocity.X).AtLeast(0);
                    var newY = (int)(node.Position.Y + m_SettingsManager.Settings.TimeStep * node.Velocity.Y).AtLeast(0);

                    node.Position = new Point(newX, newY);
                }
            }
        }

        private void FillVersions()
        {
            if (!GraphHandler.Result.Packages.Any())
                return;
            var rootNuGetPackage = GraphHandler.Result.Packages.OfType<RootNuGetPackage>().Single();
            AllFoundRootVersions = rootNuGetPackage.AvailableVersions.OrderByDescending(version => version);
            SelectedVersion = AllFoundRootVersions.FirstOrDefault(version => version == rootNuGetPackage.VersionInfo.Version);
        }

        private void RemoveOrphanedEdges()
        {
            var removeEdges = GraphEdges.Where(edge => !GraphNodes.Contains(edge.Node1) || !GraphNodes.Contains(edge.Node2)).ToArray();
            foreach (var edge in removeEdges)
            {
                GraphEdges.Remove(edge);
            }
        }

        private Point RepulsionForce(Point node1, Point node2)
        {
            double dx = node1.X - node2.X, dy = node1.Y - node2.Y;
            var sqDist = dx * dx + dy * dy;
            var d = Math.Sqrt(sqDist);
            var repulsion = m_SettingsManager.Settings.RepulsionStrength * 1.0 / sqDist;
            repulsion += -m_SettingsManager.Settings.RepulsionStrength * 0.00000006 * d;
            //clip the repulsion
            if (repulsion > m_SettingsManager.Settings.RepulsionClipping) repulsion = m_SettingsManager.Settings.RepulsionClipping;
            return new Point(repulsion * (dx / d), repulsion * (dy / d));
        }

        private void TryUpdateNodeLevel()
        {
            try
            {
                UpdateNodeLevel();
            }
            catch (Exception ex)
            {
                s_Logger.ErrorFormat("Error updating node level: {0}", ex);
            }
        }

        private void UpdateNodeLevel()
        {
            var graphNodes = GraphNodes.ToArray();
            foreach (var graphNode in graphNodes)
            {
                graphNode.ReferencedByCount = graphNodes.SelectMany(node => node.Package.Dependencies.ToArray()).Count(dep => dep == graphNode.Package.Id);
            }

            foreach (var graphNode in graphNodes)
            {
                graphNode.WeightFactor = graphNodes.Length / (double)(graphNode.ReferencedByCount + 1);
            }

            var maxWeight = graphNodes.Length > 1 ? graphNodes.Max(n => n.WeightFactor) : 1;

            foreach (var graphNode in graphNodes)
            {
                graphNode.WeightFactor = graphNode.WeightFactor / maxWeight;
            }
        }
    }
}