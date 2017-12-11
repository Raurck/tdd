using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace TagCloudLayouter
{
    [TestFixture]
    public class CircularCloudLayouter_Should
    {
        private CircularCloudLayouter layouter;
        private List<Rectangle> placedRectangles;
        private IEnumerable<Size> sizedRects;
        private int testPoolSize = 150;
        private int cloudCenterX = 1920 / 2;
        private int cloudCenterY = 1024 / 2;
        private int maxWitdh = 250;
        private int maxHeight = 60;
        private int minWitdh = 50;
        private int minHeight = 30;
        private double compactFactor = 0.5;

        private IEnumerable<Size> GenerateRectangleSizes(int rectangleCount, List<Tuple<int,int>> rectanglesList )
        {
            if (rectanglesList != null)
            {
                return rectanglesList.Select(element => new Size(element.Item1, element.Item2));
            }

            var rectangles = new List<Size>();

            var randomGenerator = new Random();
            for (var i = 0; i < rectangleCount; i++)
            {
                rectangles.Add(new Size(minWitdh + randomGenerator.Next(maxWitdh - minWitdh), minHeight + randomGenerator.Next(maxHeight - minHeight)));
            }
            return rectangles;
        }

        [SetUp]
        public void SetUp()
        {
            placedRectangles = new List<Rectangle>();
        }

        private void ParametrizeLayouter(int centerPointX, int centerPointY, int minRectangleSizeWidth, int minRectangleSizeHeight, int maxRectangleSizeWidth, int maxRectangleSizeHeight, double compactRelation, int rectanglesCount, List<Tuple<int, int>> rectanglesLis)
        {
            cloudCenterX = centerPointX;
            cloudCenterY = centerPointY;


            minWitdh = minRectangleSizeWidth;
            minHeight = minRectangleSizeHeight;
            maxWitdh = maxRectangleSizeWidth;
            maxHeight = maxRectangleSizeHeight;
            compactFactor = compactRelation;
            testPoolSize = rectanglesCount;
            layouter = new CircularCloudLayouter(new Point(cloudCenterX, cloudCenterY));
            sizedRects = GenerateRectangleSizes(testPoolSize, rectanglesLis);
        }

        private static object[] TestCases =
        {
            new object[]
            {
                1920, 1024, 
                new List<Tuple<int, int>> {new Tuple<int, int>(250, 80), new Tuple<int, int>(120, 70), new Tuple<int, int>(120, 70), new Tuple<int, int>(120, 60), new Tuple<int, int>(250, 60)}
            },
            new object[]
            {
                1920, 1024, 
                new List<Tuple<int, int>> {new Tuple<int, int>(250, 80), new Tuple<int, int>(120, 70), new Tuple<int, int>(120, 70), new Tuple<int, int>(120, 60), new Tuple<int, int>(250, 60), new Tuple<int, int>(20, 60), new Tuple<int, int>(20, 60), new Tuple<int, int>(20, 60), new Tuple<int, int>(20, 60), new Tuple<int, int>(20, 60), new Tuple<int, int>(20, 60), new Tuple<int, int>(20, 60), new Tuple<int, int>(20, 60), new Tuple<int, int>(20, 60), new Tuple<int, int>(20, 60), new Tuple<int, int>(20, 60), new Tuple<int, int>(20, 60), new Tuple<int, int>(20, 60), new Tuple<int, int>(20, 60), new Tuple<int, int>(20, 60), new Tuple<int, int>(20, 60), new Tuple<int, int>(20, 60), new Tuple<int, int>(20, 60), new Tuple<int, int>(20, 60), new Tuple<int, int>(20, 60), new Tuple<int, int>(20, 60), new Tuple<int, int>(20, 60), new Tuple<int, int>(20, 60), new Tuple<int, int>(20, 60), new Tuple<int, int>(20, 60), new Tuple<int, int>(20, 60), new Tuple<int, int>(20, 60), new Tuple<int, int>(20, 60), new Tuple<int, int>(20, 60), new Tuple<int, int>(20, 60), new Tuple<int, int>(20, 60), new Tuple<int, int>(20, 60), new Tuple<int, int>(20, 60) }
            },
            new object[]
            {
                1920, 1024,
                new List<Tuple<int, int>>
                {
                    new Tuple<int, int>(250, 80), new Tuple<int, int>(120, 70), new Tuple<int, int>(120, 70), new Tuple<int, int>(120, 60), new Tuple<int, int>(250, 60), new Tuple<int, int>(20, 60), new Tuple<int, int>(20, 60), new Tuple<int, int>(20, 60), new Tuple<int, int>(20, 60), new Tuple<int, int>(20, 60), new Tuple<int, int>(20, 60), new Tuple<int, int>(20, 60), new Tuple<int, int>(20, 60), new Tuple<int, int>(20, 60), new Tuple<int, int>(20, 60), new Tuple<int, int>(20, 60), new Tuple<int, int>(20, 60), new Tuple<int, int>(20, 60), new Tuple<int, int>(20, 60), new Tuple<int, int>(20, 60), new Tuple<int, int>(20, 60), new Tuple<int, int>(20, 60), new Tuple<int, int>(20, 60), new Tuple<int, int>(20, 60), new Tuple<int, int>(20, 60), new Tuple<int, int>(20, 60), new Tuple<int, int>(20, 60), new Tuple<int, int>(20, 60), new Tuple<int, int>(20, 60), new Tuple<int, int>(20, 60), new Tuple<int, int>(20, 60), new Tuple<int, int>(20, 60), new Tuple<int, int>(20, 60), new Tuple<int, int>(20, 60), new Tuple<int, int>(20, 60), new Tuple<int, int>(20, 60), new Tuple<int, int>(20, 60), new Tuple<int, int>(20, 60), new Tuple<int, int>(30, 20)
                    , new Tuple<int, int>(30, 20), new Tuple<int, int>(30, 20), new Tuple<int, int>(30, 20), new Tuple<int, int>(30, 20), new Tuple<int, int>(30, 20), new Tuple<int, int>(30, 20), new Tuple<int, int>(30, 20), new Tuple<int, int>(30, 20), new Tuple<int, int>(30, 20), new Tuple<int, int>(30, 20), new Tuple<int, int>(30, 20), new Tuple<int, int>(30, 20), new Tuple<int, int>(30, 20), new Tuple<int, int>(30, 20), new Tuple<int, int>(30, 20), new Tuple<int, int>(30, 20), new Tuple<int, int>(30, 20), new Tuple<int, int>(30, 20), new Tuple<int, int>(30, 20), new Tuple<int, int>(30, 20), new Tuple<int, int>(30, 20), new Tuple<int, int>(30, 20), new Tuple<int, int>(30, 20), new Tuple<int, int>(30, 20), new Tuple<int, int>(30, 20), new Tuple<int, int>(30, 20), new Tuple<int, int>(30, 20), new Tuple<int, int>(30, 20), new Tuple<int, int>(30, 20), new Tuple<int, int>(30, 20), new Tuple<int, int>(30, 20), new Tuple<int, int>(30, 20), new Tuple<int, int>(30, 20), new Tuple<int, int>(30, 20), new Tuple<int, int>(30, 20), new Tuple<int, int>(30, 20), new Tuple<int, int>(30, 20), new Tuple<int, int>(30, 20), new Tuple<int, int>(30, 20)
                }
            }

        };
        [Test, TestCaseSource(nameof(TestCases))]
        public void CloudIsShrinking_Always(int centerPointX, int centerPointY, List<Tuple<int, int>> sizeList)
        {
            
            ParametrizeLayouter(centerPointX, centerPointY, 0, 0, 0, 0, 0, 0,  sizeList);
            layouter.CompactRequired = false;
            DoLayout();
            var circumcircleArea = CalculateСircumcircleArea(layouter.GetCurrentLayout().ToList());
            SaveResults();

            ParametrizeLayouter(centerPointX, centerPointY, 0, 0, 0, 0, 0, 0,  sizeList);
            layouter.CompactRequired = true;
            DoLayout();
            var circumcircleAreaAfterShrink = CalculateСircumcircleArea(layouter.GetCurrentLayout().ToList());
            SaveResults();

            TestContext.WriteLine("Circle Area:\t\t{0}", circumcircleArea.ToString("N"));
            TestContext.WriteLine("\tCircle Radius:\t{0}", Math.Floor(Math.Sqrt(circumcircleArea / Math.PI)).ToString("N"));
            TestContext.WriteLine("Circle Area:\t\t{0}", circumcircleAreaAfterShrink.ToString("N"));
            TestContext.WriteLine("\tCircle Radius:\t{0}", Math.Floor(Math.Sqrt(circumcircleAreaAfterShrink / Math.PI)).ToString("N"));
            (circumcircleArea / circumcircleAreaAfterShrink).Should().BeGreaterThan(1);
        }

        [TestCase(1920, 1024, 50, 30, 250, 80, 0.5, 120, ExpectedResult = false)]
        [TestCase(1920, 1024, 20, 20, 20, 20, 0.6, 250, ExpectedResult = false)]
        public bool RectanglesHaveNoIntersects_Always(int centerPointX, int centerPointY, int minRectangleSizeWidth, int minRectangleSizeHeight, int maxRectangleSizeWidth, int maxRectangleSizeHeight, double compactRelation, int rectanglesCount)
        {
            ParametrizeLayouter(centerPointX, centerPointY, minRectangleSizeWidth, minRectangleSizeHeight, maxRectangleSizeWidth, maxRectangleSizeHeight, compactRelation, rectanglesCount,null);
            DoLayout();
            var hasIntersects = placedRectangles.Any(HasIntersectWithOthers);
            return hasIntersects;
        }

        [TestCase(1920, 1024, 50, 30, 250, 80, 0.5, 120, 1, 0.01, ExpectedResult = true)]
        [TestCase(1920, 1024, 50, 30, 250, 80, 0.5, 50, 1, 0.01, ExpectedResult = true)]
        [TestCase(1920, 1024, 20, 20, 20, 20, 0.6, 250, 1, ExpectedResult = true)]
        [TestCase(1920, 1024, 50, 20, 250, 80, 0.6, 250, 1, ExpectedResult = true)]
        [TestCase(1920, 1024, 20, 20, 20, 20, 0.6, 250, 10, 0.314, false, ExpectedResult = false)]
        [TestCase(1920, 1024, 20, 20, 20, 20, 0.6, 250, 15, 0.314, false, ExpectedResult = false)]
        [TestCase(1920, 1024, 20, 20, 20, 20, 0.6, 250, 5, 1.314, false, ExpectedResult = false)]
        [TestCase(1920, 1024, 20, 20, 500, 70, 0.6, 500, 4, 0.314, true, ExpectedResult = false)]
        public bool Compact(int centerPointX, int centerPointY, int minRectangleSizeWidth, int minRectangleSizeHeight, int maxRectangleSizeWidth, int maxRectangleSizeHeight, double compactRelation, int rectanglesCount, int spiralStep = 15, double spiralAngleStep = 0.314, bool doCompact = true)
        {
            ParametrizeLayouter(centerPointX, centerPointY, minRectangleSizeWidth, minRectangleSizeHeight, maxRectangleSizeWidth, maxRectangleSizeHeight, compactRelation, rectanglesCount, null);
            layouter.CompactRequired = doCompact;
            layouter.SpiralStepSize = spiralStep;
            layouter.SpiralStepAngleRadians = spiralAngleStep;
            DoLayout();
            var rectanglesCoveredArea = CalculateRectanglesArea(placedRectangles);
            var circumcircleArea = CalculateСircumcircleArea(placedRectangles);
            CoverageReport(rectanglesCoveredArea, circumcircleArea);
            return (rectanglesCoveredArea / circumcircleArea) > (compactFactor);
        }


        [TearDown]
        public void TearDown()
        {
            SaveResults();
        }


        private static void CoverageReport(double rectanglesCoveredArea, double circumcircleArea)
        {
            TestContext.WriteLine("Rectangles Area:\t{0}", rectanglesCoveredArea.ToString("N"));
            TestContext.WriteLine("Circle Area:\t\t{0}", circumcircleArea.ToString("N"));
            TestContext.WriteLine("\tCircle Radius:\t{0}", Math.Floor(Math.Sqrt(circumcircleArea / Math.PI)).ToString("N"));
            TestContext.WriteLine("Coverage:\t\t\t{0}", (rectanglesCoveredArea / circumcircleArea).ToString("N"));
        }

        private void DoLayout()
        {
            foreach (var sizedRect in sizedRects)
            {
                placedRectangles.Add(layouter.PutNextRectangle(sizedRect));
            }
        }


        private void SaveResults()
        {
            const string dateTimeFormat = "yyyyMMdd_hhmmss";
            const string subDirectory = "results";

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
            var testDateTime = DateTime.Now.ToString(dateTimeFormat);
            var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, subDirectory,
                $"{TestContext.CurrentContext.Test.MethodName}_{resultDescription}_at_{testDateTime}_{DateTime.Now.Millisecond}.png");
            var presenter = new ExtendedCloudPresenter(1920 * 2, 1024 * 2);
            presenter.PresentCloudToFile(layouter,fileName);
            
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
            return placedRectangles.Where(placedRectangle => placedRectangle != currentRectangle)
                .Any(placedRectangle => HasIntersectWithOne(currentRectangle, placedRectangle));
        }

        private double CalculateRadius(Rectangle rectangle)
        {
            var maxX = Math.Max(Math.Abs(cloudCenterX - rectangle.Left),
                Math.Abs(cloudCenterX - rectangle.Right));
            var maxY = Math.Max(Math.Abs(cloudCenterY - rectangle.Top),
                Math.Abs(cloudCenterY - rectangle.Bottom));
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