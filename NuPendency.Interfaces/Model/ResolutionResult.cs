using NuPendency.Commons.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Versioning;

namespace NuPendency.Interfaces.Model
{
    public class ResolutionResult : BaseObject
    {
        private PackageBase m_RootPackage;

        public ResolutionResult(string rootPackageName, FrameworkName targetFramework)
        {
            RootPackageName = rootPackageName;
            TargetFramework = targetFramework;
        }

        public ObservableCollection<PackageBase> Packages { get; } = new ObservableCollection<PackageBase>();

        public PackageBase RootPackage
        {
            get { return m_RootPackage; }
            set
            {
                if (m_RootPackage == value) return;
                m_RootPackage = value;
                RaisePropertyChanged();
            }
        }

        public string RootPackageName { get; }
        public FrameworkName TargetFramework { get; }
    }
}