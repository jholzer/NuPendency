using System;
using System.Linq;

namespace NuPendency.Gui.ViewModels
{
    internal static class GraphExtensions
    {
        public static bool IsNodeContainedInEdge(this GraphEdge edge, GraphNode node)
        {
            return edge.Node1 == node || edge.Node2 == node;
        }

        public static GraphNode GetOtherNode(this GraphEdge edge, GraphNode node)
        {
            return edge.Node1 == node ? edge.Node2 : edge.Node1;
        }
    }
}