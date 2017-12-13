using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TagCloudLayouter.Interfaces;


namespace TagCloudLayouter
{
    public class CircularCloudLayouter : ICloudLayouter
    {

        private readonly Point cloudCenter;
        private readonly List<Rectangle> placedRectangles = new List<Rectangle>();

        public bool CompactRequired { get; set; } = true;
        public int CanvasWidth { get; set; } = 1920;
        public int CanvasHeight { get; set; } = 1024;
        public double SpiralStepAngleRadians { get; set; } = Math.PI / 25;
        public int SpiralStepSize { get; set; } = 1;

        public double CicumcircleRadius { get; private set; } = 0;

        public Rectangle PutNextRectangle(Size rectangleSize)
        {
            if (rectangleSize.Height == 0 || rectangleSize.Width == 0)
            {
                throw new ArgumentException();
            }

            var resultRectangle = FindRectaglePlace(rectangleSize);
            if (CompactRequired)
            {
                resultRectangle = CompactCloudRegion(resultRectangle);
            }

            placedRectangles.Add(resultRectangle);

            CicumcircleRadius = Math.Max(CicumcircleRadius,
                GetCircumcircleRadiusFromLayoutCenterToRectangle(resultRectangle));

            return resultRectangle;
        }

        public IEnumerable<Rectangle> GetCurrentLayout()
        {
            return placedRectangles;
        }

        public Point GetCloudCenter()
        {
            return cloudCenter;
        }

        private double GetDistance(Point pointA, Point pointB)
        {
            return Math.Sqrt(Math.Pow(pointA.Y - pointB.Y, 2) + Math.Pow(pointA.X - pointB.X, 2));
        }

        private double GetCircumcircleRadiusFromLayoutCenterToRectangle(Rectangle rectangle)
        {
            var radius = new double[4];
            radius[0] = GetDistance(cloudCenter, new Point(rectangle.Left, rectangle.Top));
            radius[1] = GetDistance(cloudCenter, new Point(rectangle.Left, rectangle.Bottom));
            radius[2] = GetDistance(cloudCenter, new Point(rectangle.Right, rectangle.Top));
            radius[3] = GetDistance(cloudCenter, new Point(rectangle.Right, rectangle.Bottom));
            return radius.Max();
        }

        protected virtual Rectangle FindRectaglePlace(Size rectangleSize)
        {
            var startX = cloudCenter.X - rectangleSize.Width / 2;
            var startY = cloudCenter.Y - rectangleSize.Height / 2;
            var currentRectangle = new Rectangle(new Point(startX, startY), rectangleSize);
            double spiralAngleInRadian = 0;
            var spiralRadius = 0d;

            while (HasIntersectWithOthers(currentRectangle))
            {
                spiralAngleInRadian += SpiralStepAngleRadians;
                spiralRadius = SpiralStepSize * spiralAngleInRadian;
                var nextX = spiralRadius * Math.Cos(spiralAngleInRadian);
                var nextY = spiralRadius * Math.Sin(spiralAngleInRadian);
                currentRectangle.X = (int)Math.Ceiling(startX + nextX);
                currentRectangle.Y = (int)Math.Ceiling(startY + nextY);
            }
           
            return currentRectangle;
        }

        protected virtual Rectangle SeekBetterPlace(Rectangle currentRectangle)
        {
            
            var bestPlace = new Tuple<int, int, double>(currentRectangle.X, currentRectangle.Y, GetCircumcircleRadiusFromLayoutCenterToRectangle(currentRectangle));
            var maxRadius = Math.Max(CicumcircleRadius, bestPlace.Item3);
            var spiralRadius = GetDistance(cloudCenter, new Point(currentRectangle.Left, currentRectangle.Top));
            var spiralAngleInRadian = 0d;
            var startX = cloudCenter.X - currentRectangle.Width / 2;
            var startY = cloudCenter.Y - currentRectangle.Height / 2;
            var localSpiralStepAngleRadians = Math.PI / 10;
            var localSpiralStepSize = (maxRadius - spiralRadius) / 10;
            var startRadius = spiralRadius;

            while (spiralRadius < maxRadius)
            {
                spiralAngleInRadian += localSpiralStepAngleRadians;
                spiralRadius = startRadius + localSpiralStepSize * spiralAngleInRadian;
                var nextX = spiralRadius * Math.Cos(spiralAngleInRadian);
                var nextY = spiralRadius * Math.Sin(spiralAngleInRadian);
                currentRectangle.X = (int)Math.Ceiling(startX + nextX);
                currentRectangle.Y = (int)Math.Ceiling(startY + nextY);
                if (!HasIntersectWithOthers(currentRectangle) && GetCircumcircleRadiusFromLayoutCenterToRectangle(currentRectangle) < bestPlace.Item3)
                {
                    bestPlace = new Tuple<int, int, double>(currentRectangle.X, currentRectangle.Y,
                        GetCircumcircleRadiusFromLayoutCenterToRectangle(currentRectangle));
                }

            }

            currentRectangle.X = bestPlace.Item1;
            currentRectangle.Y = bestPlace.Item2;
            return currentRectangle;
        }

        protected virtual Rectangle TightenCloudRegion(Rectangle currentRectangle)
        {
            var deltaX = currentRectangle.X + currentRectangle.Width / 2 > cloudCenter.X ? -1 : 1;
            var deltaY = currentRectangle.Y + currentRectangle.Height / 2 > cloudCenter.Y ? -1 : 1;
            var lastY = currentRectangle.Y;
            var lastX = currentRectangle.X;
            var startX = lastX;
            var startY = lastY;

            while (!HasIntersectWithOthers(currentRectangle) && currentRectangle.Y != (cloudCenter.Y + deltaY * currentRectangle.Height / 2))
            {
                lastY = currentRectangle.Y;
                currentRectangle.Y = currentRectangle.Y + deltaY;
            }
            currentRectangle.Y = lastY;

            while (!HasIntersectWithOthers(currentRectangle) && currentRectangle.X != cloudCenter.X + deltaX * currentRectangle.Width / 2)
            {
                lastX = currentRectangle.X;
                currentRectangle.X = currentRectangle.X + deltaX;
            }
            currentRectangle.X = lastX;

            if (currentRectangle.X != startX || currentRectangle.Y != startY)
            {
                currentRectangle = TightenCloudRegion(currentRectangle);
            }
            return currentRectangle;
        }

        protected virtual Rectangle CompactCloudRegion(Rectangle currentRectangle)
        {
            currentRectangle = SeekBetterPlace(currentRectangle);
            currentRectangle = TightenCloudRegion(currentRectangle);
            return currentRectangle;
        }

        protected bool HasIntersectWithOthers(Rectangle currentRectangle)
        {
            return placedRectangles.Any(placedRectangle => HasIntersectWithOne(currentRectangle, placedRectangle));
        }

        protected static bool HasIntersectWithOne(Rectangle currentRectangle, Rectangle checkedRectangle)
        {
           /* return (currentRectangle.Left <= checkedRectangle.Right &&
                    checkedRectangle.Left <= currentRectangle.Right &&
                    currentRectangle.Top <= checkedRectangle.Bottom &&
                    checkedRectangle.Top <= currentRectangle.Bottom);*/
            currentRectangle.Inflate(1, 1);
            return currentRectangle.IntersectsWith(checkedRectangle);
        }

        public CircularCloudLayouter(Point center)
        {
            cloudCenter = center;
        }
    }

}