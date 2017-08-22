using System;
using System.Windows;

namespace JW.Vepix.Core.Models
{
    public class PointBoundaries
    {
        public double MinX { get; }
        public double MinY { get; }
        public double MaxX { get; }
        public double MaxY { get; }

        public PointBoundaries(Point maxPoint)
            : this(new Point(0.0, 0.0), maxPoint)
        {
        }

        public PointBoundaries(Point minPoint, Point maxPoint)
        {
            MinX = minPoint.X;
            MinY = minPoint.Y;
            MaxX = maxPoint.X;
            MaxY = maxPoint.Y;
        }

        public double GetXPointWithinBoundary(double currentXPoint) 
            => Math.Min(Math.Max(currentXPoint,MinX),
                        MaxX);

        public double GetYPointWithinBoundary(double currentYPoint)
            => Math.Min(Math.Max(currentYPoint,MinY),
                        MaxY);
    }
}
