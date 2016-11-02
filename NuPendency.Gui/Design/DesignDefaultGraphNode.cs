using NuGet;
using NuPendency.Gui.ViewModels;
using NuPendency.Interfaces.Model;
using System;

namespace NuPendency.Gui.Design
{
    public class DesignDefaultGraphNode : GraphNode
    {
        public DesignDefaultGraphNode()
        {
            Package = new NuGetPackage("TestPackage", new SemanticVersion("1.2.3.4"));
        }
    }
}