using System;
using System.Windows;

namespace NuPendency.Gui.ViewModels
{
    public static class GraphNodeForceExtensions
    {
        private const double c_AttractionConstant = 10;
        private const double c_RepulsionConstant = 1000;

        public static Vector CalcAttractionForce(this GraphNode node1, GraphNode node2, double springLength)
        {
            int proximity = (int)Math.Max(CalcDistance(node1.Position, node2.Position), 1);
            double force = c_AttractionConstant * Math.Max(proximity - springLength, 0);
            double angle = GetBearingAngle(node1.Position, node2.Position);
            return new Vector(force, angle);
        }

        public static Vector CalcRepulsionForce(this GraphNode node1, GraphNode node2)
        {
            int proximity = (int)Math.Max(CalcDistance(node1.Position, node2.Position), 1);
            double force = -(c_RepulsionConstant / Math.Pow(proximity, 2));
            double angle = GetBearingAngle(node1.Position, node2.Position);
            return new Vector(force, angle);
        }

        private static double CalcDistance(Point point1, Point point2)
        {
            return Point.Subtract(point2, point1).Length;
        }

        private static double GetBearingAngle(Point point1, Point point2)
        {
            double xDiff = point2.X - point1.X;
            double yDiff = point2.Y - point1.Y;
            return Math.Atan2(yDiff, xDiff);
        }

        public static double AtLeast(this double inValue, double minValue)
        {
            return inValue < minValue ? minValue : inValue;
        }
    }
}