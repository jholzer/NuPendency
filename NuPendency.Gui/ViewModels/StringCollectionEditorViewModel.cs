using DynamicData;
using DynamicData.Binding;
using NuPendency.Commons.Extensions;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace NuPendency.Gui.ViewModels
{
    public class DesignStringCollectionEditorViewModel : StringCollectionEditorViewModel
    {
        public DesignStringCollectionEditorViewModel() : base(new ObservableCollection<string>(new[] { "Test1", "Test2" }), null, null, "Headline")
        {
        }
    }

    public class StringCollectionEditorViewModel : ViewModelBase
    {
        private readonly Func<object, Task<Unit>> m_Add;
        private readonly Func<object, Task<Unit>> m_Remove;
        private readonly ObservableCollection<string> m_StringCollection;

        public StringCollectionEditorViewModel(ObservableCollection<string> stringCollection, Func<object, Task<Unit>> add, Func<object, Task<Unit>> remove, string headline)
        {
            m_StringCollection = stringCollection;
            m_Add = add;
            m_Remove = remove;
            Headline = headline;
        }

        public ReactiveCommand<Unit> CmdAddString { get; private set; }
        public ReactiveCommand<Unit> CmdDeleteString { get; private set; }
        public string Headline { get; }
        public IObservableCollection<string> UrlViewModel { get; } = new ObservableCollectionExtended<string>();

        public override void Init()
        {
            m_StringCollection.ToObservableChangeSet()
                .ObserveOnDispatcher()
                .Bind(UrlViewModel)
                .Subscribe()
                .AddDisposableTo(Disposables);

            CmdAddString = ReactiveCommand.CreateAsyncTask(m_Add).AddDisposableTo(Disposables);

            CmdDeleteString = ReactiveCommand.CreateAsyncTask(m_Remove).AddDisposableTo(Disposables);
        }
    }
}