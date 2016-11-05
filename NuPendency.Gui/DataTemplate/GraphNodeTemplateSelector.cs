using NuPendency.Gui.ViewModels;
using NuPendency.Interfaces.Model;
using System.Windows;
using System.Windows.Controls;

namespace NuPendency.Gui.DataTemplate
{
    public class GraphNodeTemplateSelector : DataTemplateSelector
    {
        public System.Windows.DataTemplate DefaultDataTemplate { get; set; }
        public System.Windows.DataTemplate ProjectDataTemplate { get; set; }
        public System.Windows.DataTemplate SolutionDataTemplate { get; set; }

        public override System.Windows.DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var node = item as GraphNode;
            if (node == null)
                return DefaultDataTemplate;

            if (node.Package is SolutionPackage)
                return SolutionDataTemplate;
            if (node.Package is ProjectPackage)
                return ProjectDataTemplate;
            return DefaultDataTemplate;
        }
    }
}