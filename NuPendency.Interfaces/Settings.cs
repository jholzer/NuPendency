using NuPendency.Commons.Extensions;
using NuPendency.Commons.Interfaces;
using NuPendency.Commons.Model;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reactive.Linq;

namespace NuPendency.Interfaces
{
    public class Settings : BaseObject, ISettings<Settings>
    {
        private double m_AttractionStrength;

        private double m_Damping;

        private ObservableCollection<string> m_ExcludedPackages;

        private int m_MaxSearchDepth;

        private ObservableCollection<string> m_Repositories;

        private double m_RepulsionClipping;

        private double m_RepulsionStrength;

        private double m_TimeStep;

        public Settings()
        {
            AttractionStrength = 0.2D;  //= 0.5D;
            Damping = 0.9D;             //0.9D;
            RepulsionClipping = 300D;   //200D;
            RepulsionStrength = 3000D;//1200D;
            TimeStep = 0.95;

            MaxSearchDepth = 4;
        }

        [Category("Graph settings")]
        [DisplayName("Attraction strength")]
        public double AttractionStrength
        {
            get { return m_AttractionStrength; }
            set
            {
                if (m_AttractionStrength == value) return;
                m_AttractionStrength = value;
                RaisePropertyChanged();
            }
        }

        [Category("Graph settings")]
        [DisplayName("Damping")]
        public double Damping
        {
            get { return m_Damping; }
            set
            {
                if (m_Damping == value) return;
                m_Damping = value;
                RaisePropertyChanged();
            }
        }

        [Category("Repositories")]
        [DisplayName("Excluded packages")]
        [Browsable(false)]
        public ObservableCollection<string> ExcludedPackages
        {
            get
            {
                if (m_ExcludedPackages == null)
                    CreateExcludedPackages();
                return m_ExcludedPackages;
            }
            set
            {
                if (m_ExcludedPackages == value) return;
                m_ExcludedPackages = value;
                RaisePropertyChanged();
            }
        }

        public int MaxSearchDepth
        {
            get { return m_MaxSearchDepth; }
            set
            {
                if (m_MaxSearchDepth == value) return;
                m_MaxSearchDepth = value;
                RaisePropertyChanged();
            }
        }

        [Category("Repositories")]
        [DisplayName("URLs")]
        [Browsable(false)]
        public ObservableCollection<string> Repositories
        {
            get
            {
                if (m_Repositories == null)
                    CreateRepositories();
                return m_Repositories;
            }
            set
            {
                if (m_Repositories == value) return;
                m_Repositories = value;
                RaisePropertyChanged();
            }
        }

        [Category("Graph settings")]
        [DisplayName("Repulsion clipping")]
        public double RepulsionClipping
        {
            get { return m_RepulsionClipping; }
            set
            {
                if (m_RepulsionClipping == value) return;
                m_RepulsionClipping = value;
                RaisePropertyChanged();
            }
        }

        [Category("Graph settings")]
        [DisplayName("Repulsion strength")]
        public double RepulsionStrength
        {
            get { return m_RepulsionStrength; }
            set
            {
                if (m_RepulsionStrength == value) return;
                m_RepulsionStrength = value;
                RaisePropertyChanged();
            }
        }

        [Category("Graph settings")]
        [DisplayName("Time step")]
        public double TimeStep
        {
            get { return m_TimeStep; }
            set
            {
                if (m_TimeStep == value) return;
                m_TimeStep = value;
                RaisePropertyChanged();
            }
        }

        private void CreateExcludedPackages()
        {
            m_ExcludedPackages = new ObservableCollection<string>();
            Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                    handler => handler.Invoke,
                    h => m_ExcludedPackages.CollectionChanged += h,
                    h => m_ExcludedPackages.CollectionChanged -= h)
                // ReSharper disable once ExplicitCallerInfoArgument
                .Subscribe(_ => RaisePropertyChanged(nameof(ExcludedPackages)))
                .AddDisposableTo(Disposables);
        }

        private void CreateRepositories()
        {
            m_Repositories = new ObservableCollection<string>();
            Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                    handler => handler.Invoke,
                    h => m_Repositories.CollectionChanged += h,
                    h => m_Repositories.CollectionChanged -= h)
                // ReSharper disable once ExplicitCallerInfoArgument
                .Subscribe(_ => RaisePropertyChanged(nameof(Repositories)))
                .AddDisposableTo(Disposables);
        }
    }
}