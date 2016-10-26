using ReactiveUI;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace NuPendency.Commons.Ui
{
    public class DesignInputBoxViewModel : InputBoxViewModel
    {
        public DesignInputBoxViewModel() : base("The message to be displayed for textinput", text => { }, text => { })
        {
        }
    }

    public class InputBoxViewModel : DialogViewModelBase
    {
        private readonly Action<string> m_CancelAction;
        private readonly Action<string> m_OkAction;
        private string m_InputText;

        public InputBoxViewModel(string text, Action<string> okAction, Action<string> cancelAction)
        {
            m_OkAction = okAction;
            m_CancelAction = cancelAction;
            Message = text;
        }

        public ReactiveCommand<bool> CmdCancel { get; private set; }
        public ReactiveCommand<bool> CmdOk { get; private set; }

        public string InputText
        {
            get { return m_InputText; }
            set
            {
                if (value == m_InputText) return;
                m_InputText = value;
                raisePropertyChanged();
            }
        }

        public string Message { get; }

        public override void Init()
        {
            // ReSharper disable once InvokeAsExtensionMethod
            IObservable<bool> canOk = Observable.CombineLatest(
                Observable.Return(m_OkAction != null),
                this.WhenAnyValue(model => model.InputText).Select(txt => !string.IsNullOrEmpty(txt)),
                (ok, txt) => ok && txt);

            CmdOk = ReactiveCommand.CreateAsyncTask(canOk, _ =>
            {
                DialogCompletedTask.RunSynchronously();
                Task.Run(() =>
                {
                    m_OkAction.Invoke(InputText);
                });
                return Task.FromResult(true);
            });

            CmdCancel = ReactiveCommand.CreateAsyncTask(Observable.Return(m_CancelAction != null), _ =>
            {
                DialogCompletedTask.RunSynchronously();
                Task.Run(() => m_CancelAction.Invoke(InputText));
                return Task.FromResult(true);
            });
        }
    }
}