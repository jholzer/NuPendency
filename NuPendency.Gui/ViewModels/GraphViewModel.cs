using DynamicData;
using DynamicData.Binding;
using log4net;
using NuPendency.Commons.Extensions;
using NuPendency.Commons.Interfaces;
using NuPendency.Gui.Design;
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
using System.Windows;

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

        public Settings Settings => m_SettingsManager.Settings;

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
                .Do(_ => RemoveOrphanedEdges())
                .Subscribe()
                .AddDisposableTo(Disposables);
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