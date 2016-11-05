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
            Package = new NuGetPackage("TestPackage", new SemanticVersion("1.2.3.4"), new[] { new Version(1, 2, 3, 4) });
        }
    }

    public class DesignProjectGraphNode : GraphNode
    {
        public DesignProjectGraphNode()
        {
            Package = new ProjectPackage("TestProject", new SemanticVersion("1.2.3.4"));
        }
    }

    public class DesignSolutiontGraphNode : GraphNode
    {
        public DesignSolutiontGraphNode()
        {
            Package = new SolutionPackage("TestSolution", new SemanticVersion("1.2.3.4"));
        }
    }
}