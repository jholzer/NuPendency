using System;
using System.ComponentModel;
using System.IO;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using NuPendency.Commons.Extensions;
using NuPendency.Commons.Interfaces;
using NuPendency.Interfaces.Annotations;

namespace NuPendency.Commons.Model
{
    public abstract class BaseObject : INotifyPropertyChanged, IDisposable
    {
        protected CompositeDisposable Disposables = new CompositeDisposable();

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void Dispose()
        {
            Disposables?.Dispose();
            Disposables = null;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class SettingsManager<T> : ISettingsManager<T> where T : ISettings<T>
    {
        private readonly CompositeDisposable m_Disposable = new CompositeDisposable();

        public SettingsManager(ISettingsRootProvider<T> rootProvider)
        {
            var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string settingsFile = Path.Combine(folderPath, "NuPendencySettings.json");

            if (File.Exists(settingsFile))
            {
                Settings = JsonConvert.DeserializeObject<T>(File.ReadAllText(settingsFile));
            }
            else
            {
                Settings = rootProvider.CreateDefaultSettings();
                SaveSeetings(settingsFile);
            }

            Settings.OnAnyPropertyChanges().Do(_ =>
            {
                SaveSeetings(settingsFile);
            })
                .Subscribe()
                .AddDisposableTo(m_Disposable);
        }

        public T Settings { get; }

        private void SaveSeetings(string settingsFile)
        {
            var serializeObject = JsonConvert.SerializeObject(Settings);
            File.WriteAllText(settingsFile, serializeObject);
        }
    }
}