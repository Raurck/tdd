using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TagCloud.Interfaces;

namespace TagCloud
{
    public class ExtendedCloudPresenter : CloudPresenter
    {

        public ExtendedCloudPresenter(int canvasWidth, int canvasHeight) : base(canvasWidth, canvasHeight)
        {
        }


        private double CalculateСircumcircleRadius(IEnumerable<Rectangle> rectangles, Point center)
        {
            return rectangles.Select(rect => CalculateRadius(rect, center)).Max();
        }

        private double CalculateRadius(Rectangle rectangle, Point center)
        {
            var maxX = Math.Max(Math.Abs(center.X - rectangle.Left),
                Math.Abs(center.X - rectangle.Right));
            var maxY = Math.Max(Math.Abs(center.Y - rectangle.Top),
                Math.Abs(center.Y - rectangle.Bottom));
            return Math.Sqrt(maxX * maxX + maxY * maxY);
        }

        protected void DrawCircle(Graphics canvas, ICloudLayouter layouter)
        {
            var centerPoint = layouter.GetCloudCenter();
            var radius = (int) Math.Ceiling(CalculateСircumcircleRadius(layouter.GetCurrentLayout(), centerPoint));
            canvas.DrawEllipse(new Pen(Color.ForestGreen, 2), centerPoint.X - radius, centerPoint.Y - radius, 2 * radius, 2 * radius);
        }

        public override void PresentCloudToFile(ICloudLayouter layouter, string fileName)
        {
            fileName = GetRealFileName(fileName);
            using (var presentation = new Bitmap(canvasWidth, canvasHeight))
            {
                using (var canvas = Graphics.FromImage(presentation))
                {
                    DrawRectangles(canvas, layouter.GetCurrentLayout());
                    DrawCircle(canvas, layouter);
                }
                presentation?.Save(fileName);
            }
        }
    }
}