using System;
using System.Linq;

namespace NuPendency.Gui.ViewModels
{
    internal static class GraphExtensions
    {
        public static bool IsNodeContainedInEdge(this GraphEdge edge, GraphNode node)
        {
            return edge.Node == node || edge.DependingNode == node;
        }

        public static GraphNode GetOtherNode(this GraphEdge edge, GraphNode node)
        {
            return edge.Node == node ? edge.DependingNode : edge.Node;
        }
    }
}