using DynamicData;
using DynamicData.Binding;
using Microsoft.Win32;
using NuPendency.Commons.Extensions;
using NuPendency.Commons.Interfaces;
using NuPendency.Commons.Ui;
using NuPendency.Interfaces;
using NuPendency.Interfaces.Model;
using NuPendency.Interfaces.Services;
using ReactiveUI;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace NuPendency.Gui.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IGraphManager m_GraphManager;
        private readonly ISettingsManager<Settings> m_SettingsManager;
        private readonly IViewModelFactory m_ViewModelFactory;
        private DialogViewModelBase m_DialogViewModel;
        private GraphViewModel m_SelectedGraph;

        public MainWindowViewModel(IViewModelFactory viewModelFactory,
            IGraphManager graphManager,
            ISettingsManager<Settings> settingsManager)
        {
            m_ViewModelFactory = viewModelFactory;
            m_GraphManager = graphManager;
            m_SettingsManager = settingsManager;
        }

        public ReactiveCommand<Unit> CmdAddNewGraph { get; private set; }
        public ReactiveCommand<Unit> CmdCancel { get; private set; }
        public ReactiveCommand<Unit> CmdDeleteGraph { get; private set; }
        public ReactiveCommand<Unit> CmdOpenSolution { get; private set; }
        public ReactiveCommand<Unit> CmdRefreshGraph { get; private set; }

        public DialogViewModelBase DialogViewModel
        {
            get { return m_DialogViewModel; }
            set
            {
                m_DialogViewModel = value;
                raisePropertyChanged();
            }
        }

        public StringCollectionEditorViewModel ExclusionsEditorViewModel { get; private set; }

        public ObservableCollectionExtended<GraphViewModel> Graphs { get; } = new ObservableCollectionExtended<GraphViewModel>();

        public StringCollectionEditorViewModel RepositoryEditorViewModel { get; private set; }

        public GraphViewModel SelectedGraph
        {
            get { return m_SelectedGraph; }
            set
            {
                m_SelectedGraph = value;
                raisePropertyChanged();
            }
        }

        public Settings Settings => m_SettingsManager.Settings;

        public override void Init()
        {
            var addRepo = new Func<object, Task<Unit>>(o =>
            {
                Action<string> okAction = url =>
                {
                    m_SettingsManager.Settings.Repositories.Add(url);
                };
                var inputBoxVm = m_ViewModelFactory.CreateInputBoxViewModel("Enter URL of new repository", okAction, s => { });
                ShowDialog(inputBoxVm);
                return Task.FromResult(Unit.Default);
            });

            var removeRepo = new Func<object, Task<Unit>>(o =>
            {
                string url = o as string;
                m_SettingsManager.Settings.Repositories.Remove(url);
                return Task.FromResult(Unit.Default);
            });

            RepositoryEditorViewModel = new StringCollectionEditorViewModel(m_SettingsManager.Settings.Repositories, addRepo, removeRepo, "Repositories");
            RepositoryEditorViewModel.Init();

            var addExclusion = new Func<object, Task<Unit>>(o =>
            {
                Action<string> okAction = packName =>
                {
                    m_SettingsManager.Settings.ExcludedPackages.Add(packName);
                };
                var inputBoxVm = m_ViewModelFactory.CreateInputBoxViewModel("Enter name of excluded package", okAction, s => { });
                ShowDialog(inputBoxVm);
                return Task.FromResult(Unit.Default);
            });

            var removeExclusion = new Func<object, Task<Unit>>(o =>
            {
                string packName = o as string;
                m_SettingsManager.Settings.ExcludedPackages.Remove(packName);
                return Task.FromResult(Unit.Default);
            });

            ExclusionsEditorViewModel = new StringCollectionEditorViewModel(m_SettingsManager.Settings.ExcludedPackages, addExclusion, removeExclusion, "Excluded packages");
            ExclusionsEditorViewModel.Init();

            m_GraphManager.Documents.ToObservableChangeSet()
                .ObserveOnDispatcher()
                .Transform(CreateGraphViewModel)
                .Bind(Graphs)
                .Do(set =>
                {
                    var change = set.FirstOrDefault();
                    if (change == null)
                        return;

                    if (set.Adds > 0)
                        SwitchTo(change.Item.Current);
                    else if (set.Removes > 0)
                        ClearView();
                })
                .DisposeMany()
                .Subscribe()
                .AddDisposableTo(Disposables);

            //m_GraphManager.Documents.ToObservableChangeSet(handler => handler == SelectedGraph?.GraphHandler)
            //    .WhereReasonsAre(ChangeReason.Remove)
            //    .ObserveOnDispatcher()
            //    .Do(set => SelectedGraph = null)
            //    .Subscribe()
            //    .AddDisposableTo(Disposables);

            CmdAddNewGraph = ReactiveCommand.CreateAsyncTask(_ =>
            {
                Action<string> okaction = name =>
                {
                    var graphHandler = m_GraphManager.CreateNewDocument(name);

                    Task.Run(() =>
                    {
                        graphHandler.ResolveDependencies();
                    });
                };

                Action<string> cancelAction = txt => { };
                var inputVm = m_ViewModelFactory.CreateInputBoxViewModel("Enter name of Packge to investigate", okaction, cancelAction);

                ShowDialog(inputVm);

                return Task.FromResult(Unit.Default);
            }).AddDisposableTo(Disposables);

            CmdOpenSolution = ReactiveCommand.CreateAsyncTask(_ =>
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "VS Solution (*.sln) | *.sln",
                    Multiselect = false
                };
                var result = openFileDialog.ShowDialog();

                if (!result.HasValue || !result.Value)
                    return Task.FromResult(false);

                var graphHandler = m_GraphManager.CreateNewDocument(openFileDialog.FileName);
                Task.Run(() =>
                {
                    graphHandler.ResolveDependencies();
                });

                return Task.FromResult(Unit.Default);
            }).AddDisposableTo(Disposables);

            CmdRefreshGraph = ReactiveCommand.CreateAsyncTask(o =>
            {
                var result = o as ResolutionResult;
                var graphHandler = m_GraphManager.Documents.FirstOrDefault(handler => handler.Result == result);
                if (graphHandler == null)
                    return Task.FromResult(Unit.Default);

                Task.Run(() =>
                {
                    graphHandler.ResolveDependencies();
                });
                return Task.FromResult(Unit.Default);
            }).AddDisposableTo(Disposables);

            CmdCancel = ReactiveCommand.CreateAsyncTask(o =>
            {
                var result = o as ResolutionResult;
                var graphHandler = m_GraphManager.Documents.FirstOrDefault(handler => handler.Result == result);
                if (graphHandler == null)
                    return Task.FromResult(Unit.Default);

                graphHandler.Cancel();

                return Task.FromResult(Unit.Default);
            }).AddDisposableTo(Disposables);

            CmdDeleteGraph = ReactiveCommand.CreateAsyncTask(o =>
            {
                var result = o as ResolutionResult;
                var graphHandler = m_GraphManager.Documents.FirstOrDefault(handler => handler.Result == result);
                if (graphHandler == null)
                    return Task.FromResult(Unit.Default);

                m_GraphManager.Delete(graphHandler);

                return Task.FromResult(Unit.Default);
            }).AddDisposableTo(Disposables);
        }

        public async void ShowDialog(DialogViewModelBase dialogViewModel)
        {
            DialogViewModel = dialogViewModel;
            await DialogViewModel.DialogCompletedTask;
            DialogViewModel = null;
        }

        private void ClearView()
        {
            SelectedGraph = null;
        }

        private GraphViewModel CreateGraphViewModel(IGraphHandler doc)
        {
            return m_ViewModelFactory.CreateViewModel<IGraphHandler, GraphViewModel>(doc);
        }

        private void SwitchTo(GraphViewModel graphViewModel)
        {
            if (graphViewModel == null)
                return;
            SelectedGraph = graphViewModel;
        }
    }
}