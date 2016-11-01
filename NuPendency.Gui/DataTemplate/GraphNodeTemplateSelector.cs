using NuPendency.Gui.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace NuPendency.Gui.DataTemplate
{
    public class GraphNodeTemplateSelector : DataTemplateSelector
    {
        public System.Windows.DataTemplate DefaultDataTemplate { get; set; }

        public override System.Windows.DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return DefaultDataTemplate;
        }
    }
}