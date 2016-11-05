using NuPendency.Commons.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Versioning;

namespace NuPendency.Interfaces.Model
{
    public class ResolutionResult : BaseObject
    {
        private Guid m_RootPackageId;

        public ResolutionResult(string rootPackageName, FrameworkName targetFramework)
        {
            RootPackageName = rootPackageName;
            TargetFramework = targetFramework;
        }

        public ObservableCollection<PackageBase> Packages { get; } = new ObservableCollection<PackageBase>();

        public Guid RootPackageId
        {
            get { return m_RootPackageId; }
            set
            {
                if (m_RootPackageId == value) return;
                m_RootPackageId = value;
                RaisePropertyChanged();
            }
        }

        public string RootPackageName { get; }
        public FrameworkName TargetFramework { get; }
    }
}