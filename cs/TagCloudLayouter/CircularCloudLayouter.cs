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

        protected virtual Rectangle FindRectaglePlace(Size rectangleSize)
        {
            var startX = cloudCenter.X - rectangleSize.Width / 2;
            var startY = cloudCenter.Y - rectangleSize.Height / 2;
            var currentRectangle = new Rectangle(new Point(startX, startY), rectangleSize);
            double spiralAngleInRadian = 0;

            while (HasIntersectWithOthers(currentRectangle))
            {
                spiralAngleInRadian += SpiralStepAngleRadians;
                var spiralRadius = SpiralStepSize * spiralAngleInRadian;
                var nextX = spiralRadius * Math.Cos(spiralAngleInRadian);
                var nextY = spiralRadius * Math.Sin(spiralAngleInRadian);
                currentRectangle.X = (int)Math.Ceiling(startX + nextX);
                currentRectangle.Y = (int)Math.Ceiling(startY + nextY);
            }
            return currentRectangle;
        }

        protected virtual Rectangle CompactCloudRegion(Rectangle currentRectangle)
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
                currentRectangle = CompactCloudRegion(currentRectangle);
            }
            return currentRectangle;
        }

        protected bool HasIntersectWithOthers(Rectangle currentRectangle)
        {
            return placedRectangles.Any(placedRectangle => HasIntersectWithOne(currentRectangle, placedRectangle));
        }

        protected static bool HasIntersectWithOne(Rectangle currentRectangle, Rectangle checkedRectangle)
        {
            return (currentRectangle.Left <= checkedRectangle.Right &&
                    checkedRectangle.Left <= currentRectangle.Right &&
                    currentRectangle.Top <= checkedRectangle.Bottom &&
                    checkedRectangle.Top <= currentRectangle.Bottom);
        }

        public CircularCloudLayouter(Point center)
        {
            cloudCenter = center;
        }
    }

}