using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using TagCloudLayouter.Interfaces;

namespace TagCloudLayouter
{
    public class CircularCloudLayouter : ICloudLayouter
    {
        private const int CanvasWidth = 1920;
        private const int CanvasHeight = 1024;
        private const double DeltaAngleDegree = Math.PI / 25;
        private const int SpiralStepSize = 1;

        private readonly Point cloudCenter;
        private readonly List<Rectangle> placedRectangles = new List<Rectangle>();

        public Rectangle PutNextRectangle(Size rectangleSize)
        {
            var resultRectangle = FindRectaglePlace(rectangleSize);
            resultRectangle = CompactRegion(resultRectangle);
            placedRectangles.Add(resultRectangle);
            return resultRectangle;
        }

        private Rectangle FindRectaglePlace(Size rectangleSize)
        {
            var startX = cloudCenter.X - rectangleSize.Width / 2;
            var startY = cloudCenter.Y - rectangleSize.Height / 2;
            var currentRectangle = new Rectangle(new Point(startX, startY), rectangleSize);
            double spiralAngleInRadian = 0;

            while (HasIntersectWithOthers(currentRectangle))
            {
                spiralAngleInRadian += DeltaAngleDegree;
                var spiralRadius = SpiralStepSize * spiralAngleInRadian;
                var nextX = spiralRadius * Math.Cos(spiralAngleInRadian);
                var nextY = spiralRadius * Math.Sin(spiralAngleInRadian);
                currentRectangle.X = (int)Math.Ceiling(startX + nextX);
                currentRectangle.Y = (int)Math.Ceiling(startY + nextY);
            }
            return currentRectangle;
        }

        private Rectangle CompactRegion(Rectangle currentRectangle)
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
                currentRectangle = CompactRegion(currentRectangle);
            }
            return currentRectangle;
        }

        private bool HasIntersectWithOthers(Rectangle currentRectangle)
        {
            return placedRectangles.Any(placedRectangle => HasIntersectWithOne(currentRectangle, placedRectangle));
        }

        private static bool HasIntersectWithOne(Rectangle currentRectangle, Rectangle checkedRectangle)
        {
            return (currentRectangle.Left <= checkedRectangle.Right &&
                    checkedRectangle.Left <= currentRectangle.Right &&
                    currentRectangle.Top <= checkedRectangle.Bottom &&
                    checkedRectangle.Top <= currentRectangle.Bottom);
        }

        public Bitmap Draw()
        {
            var result = new Bitmap(CanvasWidth, CanvasHeight);
            using (var canvas = Graphics.FromImage(result))
            {
                canvas.DrawRectangles(new Pen(Color.Blue, 2), placedRectangles.ToArray());
            }
            
            return result;
        }

        public void Save(string fileName)
        {
            if (String.IsNullOrWhiteSpace(fileName))
            {
                fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "results", "rectangles.png");
            }

            var resultDirectory = Path.GetDirectoryName(fileName);
            if (String.IsNullOrWhiteSpace(resultDirectory))
            {
                resultDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "results");
                fileName = Path.Combine(resultDirectory, "rectangles.png");
            }

            if (!Directory.Exists(resultDirectory))
            {
                Directory.CreateDirectory(resultDirectory);
            }

            using (var bitmap = Draw())
            {
                bitmap?.Save(fileName);
            }
        }

        public CircularCloudLayouter(Point center)
        {
            cloudCenter = center;
        }
    }

    [TestFixture]
    internal class CircularCloudLayouterSould
    {
        private CircularCloudLayouter layouter;
        private List<Rectangle> placedRectangles;

        private const int TestPoolSize = 500;
        private const int CloudCenterX = 250;
        private const int CloudCenterY = 250;
        private const int MaxWitdh = 200;
        private const int MaxHeight = 60;
        private const double CompactFactor = 0.4;

        private static IEnumerable<Size> GenerateRectangleSizes(int rectangleCount)
        {
            var rectangles = new List<Size>();
            var randomGenerator = new Random();
            for (var i = 0; i < rectangleCount; i++)
            {
                rectangles.Add(new Size(randomGenerator.Next(MaxWitdh), randomGenerator.Next(MaxHeight)));
            }
            return rectangles;
        }

        [SetUp]
        public void SetUp()
        {
            layouter = new CircularCloudLayouter(new Point(CloudCenterX, CloudCenterY));
            var sizedRects = GenerateRectangleSizes(TestPoolSize);
            placedRectangles = new List<Rectangle>();
            foreach (var sizedRect in sizedRects)
            {
                placedRectangles.Add(layouter.PutNextRectangle(sizedRect));
            }
        }

        [Test]
        public void RectanglesHaveNoIntersects()
        {
            var hasIntersects = placedRectangles.Any(HasIntersectWithOthers);
            Assert.AreEqual(false, hasIntersects, "Rectangles have intersects with each others");
        }

        [Test]
        public void CloudIsCompactEnougth()
        {
            var rectanglesCoveredArea = CalculateRectanglesArea(placedRectangles);
            var circumcircleArea = CalculateСircumcircleArea(placedRectangles);
            Assert.GreaterOrEqual(rectanglesCoveredArea / circumcircleArea, CompactFactor, "The cloud is not compact enogth");
        }

        [TearDown]
        public void TearDown()
        {
            SaveResults();          
        }

        private void SaveResults()
        {
            var dateTimeFormat = "yyyyMMdd_hhmmss";
            var resultDescription = "other";
            switch (TestContext.CurrentContext.Result.Outcome.Status)
            {
                case TestStatus.Failed:
                    resultDescription = "failed";
                    break;
                case TestStatus.Passed:
                    resultDescription = "success";
                    break;
            }

            layouter.Save(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "results",
                $"{TestContext.CurrentContext.Test.MethodName}_{resultDescription}_at_{DateTime.Now.ToString(dateTimeFormat)}.png"));
        }

        private static double CalculateRectanglesArea(List<Rectangle> rectangles)
        {
            return rectangles.Select(CalculateRectangleArea).Sum();
        }

        private static int CalculateRectangleArea(Rectangle rectangle)
        {
            return rectangle.Height * rectangle.Width;
        }

        private double CalculateСircumcircleArea(List<Rectangle> rectangles)
        {
            var сircumcircleRadius = CalculateСircumcircleRadius(rectangles);
            return сircumcircleRadius * сircumcircleRadius * Math.PI;
        }

        private double CalculateСircumcircleRadius(List<Rectangle> rectangles)
        {
            return rectangles.Select(CalculateRadius).Max();
        }

        private bool HasIntersectWithOthers(Rectangle currentRectangle)
        {
            return placedRectangles.Where(placedRectangle=> placedRectangle!=currentRectangle).Any(placedRectangle => HasIntersectWithOne(currentRectangle, placedRectangle));
        }

        private static double CalculateRadius(Rectangle rectangle)
        {
            var maxX = Math.Max(Math.Abs(CloudCenterX - rectangle.Left),
                Math.Abs(CloudCenterX - rectangle.Right));
            var maxY = Math.Max(Math.Abs(CloudCenterY - rectangle.Top),
                Math.Abs(CloudCenterY - rectangle.Bottom));
            return Math.Sqrt(maxX * maxX + maxY * maxY);
        }

        private static bool HasIntersectWithOne(Rectangle currentRectangle, Rectangle checkinRectangle)
        {
            return (currentRectangle.Left <= checkinRectangle.Right &&
                    checkinRectangle.Left <= currentRectangle.Right &&
                    currentRectangle.Top <= checkinRectangle.Bottom &&
                    checkinRectangle.Top <= currentRectangle.Bottom);
        }

       
    }
}