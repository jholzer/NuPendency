using System;
using System.Reactive.Disposables;

namespace NuPendency.Commons.Extensions
{
    public static class DisposableExtensions
    {
        public static T AddDisposableTo<T>(this T obj, CompositeDisposable disposables) where T : IDisposable
        {
            disposables.Add(obj);
            return obj;
        }
    }
}