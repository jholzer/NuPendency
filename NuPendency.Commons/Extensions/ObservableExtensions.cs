using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using NuPendency.Commons.Interfaces;
using NuPendency.Interfaces.Annotations;
using ReactiveUI;

namespace NuPendency.Commons.Extensions
{
    public static class ObservableExtensions
    {
        public static IObservable<T> OnAnyPropertyChanges<T>(this T source) where T : INotifyPropertyChanged
        {
            return Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                handler => handler.Invoke,
                h => source.PropertyChanged += h,
                h => source.PropertyChanged -= h)
                .Select(_ => source);
        }

        public static IObservable<TR> ToObservable<T, TR>(this T target, Expression<Func<T, TR>> property) where T : INotifyPropertyChanged
        {
            var f = property.Body as MemberExpression;

            if (f == null)
                throw new NotSupportedException("Only use expressions that call a single property");

            var propertyName = f.Member.Name;
            var getValueFunc = property.Compile();
            return Observable.Create<TR>(o =>
            {
                PropertyChangedEventHandler eventHandler = (s, pce) =>
                {
                    if (pce.PropertyName == null || pce.PropertyName == propertyName)
                        o.OnNext(getValueFunc(target));
                };
                target.PropertyChanged += eventHandler;
                return () => target.PropertyChanged -= eventHandler;
            });
        }

        public static IObservable<T> ToObservable<T>(this T source) where T : INotifyPropertyChanged
        {
            return Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                handler => handler.Invoke,
                h => source.PropertyChanged += h,
                h => source.PropertyChanged -= h)
                .Select(_ => source);
        }
    }

    public abstract class ViewModelBase : ReactiveObject, IDisposable, IInitializable
    {
        protected CompositeDisposable Disposables = new CompositeDisposable();
        private bool m_Disposed;

        ~ViewModelBase()
        {
            Dispose(false);
        }

        public virtual void Dispose()
        {
            Dispose(true);
        }

        public abstract void Init();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "Disposables")]
        protected virtual void Dispose(bool disposing)
        {
            if (m_Disposed)
                return;

            Disposables?.Dispose();
            Disposables = null;

            m_Disposed = true;
        }

        [NotifyPropertyChangedInvocator]
        // ReSharper disable once InconsistentNaming
        protected void raisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            this.RaisePropertyChanged(propertyName);
        }
    }
}