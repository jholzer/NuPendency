using DynamicData;
using DynamicData.Binding;
using emvoll.commons.Extensions;
using emvoll.commons.Interfaces;
using emvoll.commons.Ui;
using NuPendency.Gui.Design;
using NuPendency.Interfaces;
using ReactiveUI;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace NuPendency.Gui.ViewModels
{
    public class DesignRepositoryViewModel : RepositoryViewModel
    {
        public DesignRepositoryViewModel() : base(new DesignSettingsManager(), null, null)
        {
        }
    }

    public class RepositoryViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel m_MainWindowViewModel;
        private readonly ISettingsManager<Settings> m_SettingsManager;
        private readonly IViewModelFactory m_ViewModelFactory;

        public RepositoryViewModel(ISettingsManager<Settings> settingsManager, MainWindowViewModel mainWindowViewModel, IViewModelFactory viewModelFactory)
        {
            m_SettingsManager = settingsManager;
            m_MainWindowViewModel = mainWindowViewModel;
            m_ViewModelFactory = viewModelFactory;
        }

        public ReactiveCommand<Unit> CmdAddRepository { get; private set; }
        public ReactiveCommand<Unit> CmdDeleteRepository { get; private set; }

        public IObservableCollection<RepoUrlViewMode> UrlViewModel { get; } =
            new ObservableCollectionExtended<RepoUrlViewMode>();

        public override void Init()
        {
            m_SettingsManager.Settings.Repositories
                .ToObservableChangeSet()
                .Transform(CreateRepoUrlViewMode)
                .ObserveOnDispatcher()
                .Bind(UrlViewModel)
                .Subscribe()
                .AddDisposableTo(Disposables);

            CmdAddRepository = ReactiveCommand.CreateAsyncTask(_ =>
            {
                Action<string> okAction = url =>
                {
                    m_SettingsManager.Settings.Repositories.Add(url);
                };
                var inputBoxVm = m_ViewModelFactory.CreateInputBoxViewModel("Enter URL of new repository", okAction, s => { });
                m_MainWindowViewModel.ShowDialog(inputBoxVm);
                return Task.FromResult(Unit.Default);
            });

            CmdDeleteRepository = ReactiveCommand.CreateAsyncTask(repoUrlVm =>
            {
                var vm = repoUrlVm as RepoUrlViewMode;
                m_SettingsManager.Settings.Repositories.Remove(vm.Url);
                return Task.FromResult(Unit.Default);
            }).AddDisposableTo(Disposables);
        }

        private static RepoUrlViewMode CreateRepoUrlViewMode(string url)
        {
            return new RepoUrlViewMode(url);
        }
    }
}