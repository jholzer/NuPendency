using NuPendency.Commons.Ui;
using System;

namespace NuPendency.Commons.Interfaces
{
    public interface IViewModelFactory
    {
        T Create<T>();

        InputBoxViewModel CreateInputBoxViewModel(string text, Action<string> okAction, Action<string> cancelAction);

        TVm CreateViewModel<T, TVm>(T model);

        TVm CreateViewModel<TVm>();
    }
}