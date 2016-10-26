using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Layout;
using Ninject;
using NuPendency.Commons.Interfaces;
using NuPendency.Core;
using NuPendency.Gui;
using NuPendency.Gui.ViewModels;
using NuPendency.Gui.Views;
using System;
using System.Reflection;
using System.Windows;
using App = NuPendency.Gui.App;

namespace NuPendency
{
    public static class ApplicationExtensions
    {
        public static void ReplaceViewModelLocator(this Application application, IViewModelFactory viewModelLocator, string locatorKey = "Locator")
        {
            if (application.Resources.Contains(locatorKey))
                application.Resources.Remove(locatorKey);
            application.Resources.Add(locatorKey, viewModelLocator);
        }
    }

    internal class Program
    {
        private static ILog s_Logger;

        private static Application CreateApplication(IViewModelFactory viewModelLocator)
        {
            var application = new App();

            application.InitializeComponent();
            application.ReplaceViewModelLocator(viewModelLocator);

            return application;
        }

        private static void LoadModules(IKernel kernel)
        {
            kernel.Load<GuiModuleCatalog>();
            kernel.Load<CoreModuleCatalog>();
        }

        [STAThread]
        private static void Main()
        {
            using (IKernel kernel = new StandardKernel())
            {
                LoadModules(kernel);

                var x = new ConsoleAppender { Layout = new SimpleLayout() };
                BasicConfigurator.Configure(x);

                s_Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

                var viewModelFactory = kernel.Get<ViewModelLocator>();
                var application = CreateApplication(viewModelFactory);

                var mainWindowViewModel = viewModelFactory.CreateViewModel<MainWindowViewModel>();
                s_Logger.Info("Initializing application");

                var mainWindow = kernel.Get<MainWindow>();
                mainWindow.DataContext = mainWindowViewModel;

                application.Run(mainWindow);
                application.Shutdown();
            }
        }
    }
}