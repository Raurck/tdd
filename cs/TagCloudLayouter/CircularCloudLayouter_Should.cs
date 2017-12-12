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
        private int cloudCenterX = 1920 / 2;
        private int cloudCenterY = 1024 / 2;

        [SetUp]
        public void SetUp()
        {
            placedRectangles = new List<Rectangle>();
        }

        private static IEnumerable<Size> GenerateRectangleSizes(int rectangleCount, int minRectangleWidth = 50, int minRectangleHeight = 25, int maxRectangleWidth = 300, int maxRectangleHeight = 90)
        {
            var rectangles = new List<Size>();

            var randomGenerator = new Random();
            for (var i = 0; i < rectangleCount; i++)
            {
                rectangles.Add(new Size(minRectangleWidth + randomGenerator.Next(maxRectangleWidth - minRectangleWidth), minRectangleHeight + randomGenerator.Next(maxRectangleHeight - minRectangleHeight)));
            }
            return rectangles;
        }

        private void ParametrizeLayouter(int centerPointX, int centerPointY, IEnumerable<Size> rectanglesLis, int rectanglesCount = 200, int minRectangleSizeWidth = 50, int minRectangleSizeHeight = 25, int maxRectangleSizeWidth = 300, int maxRectangleSizeHeight = 90)
        {
            cloudCenterX = centerPointX;
            cloudCenterY = centerPointY;

            layouter = new CircularCloudLayouter(new Point(cloudCenterX, cloudCenterY));
            sizedRects = rectanglesLis ?? GenerateRectangleSizes(rectanglesCount, minRectangleSizeWidth, minRectangleSizeHeight, maxRectangleSizeWidth, maxRectangleSizeHeight);
        }

        private static object[] _testCases =
        {
            new object[]
            {
                1920, 1024,
                new List<Size>
                {
                    new Size(500,90),
                    new Size(169,80),
                    new Size(285,80),
                    new Size(185,80),
                    new Size(400,80),
                    new Size(250,75),
                    new Size(180,75),
                    new Size(310,75),
                    new Size(201,75),
                    new Size(220,75),
                    new Size(250,75),
                    new Size(186,60),
                    new Size(178,60),
                    new Size(135,60),
                    new Size(150,60),
                    new Size(140,60),
                    new Size(190,60),
                    new Size(175,60),
                    new Size(155,50),
                    new Size(143,50),
                    new Size(122,50),
                    new Size(100,50),
                    new Size(98,50),
                    new Size(147,50),
                    new Size(115,50),
                    new Size(123,50),
                    new Size(200,50),
                    new Size(135,50),
                    new Size(141,50)
                }
            },
            new object[]{1920, 1024, null},
            new object[]{1920, 1024, null},
            new object[]{1920, 1024, null},
            new object[]{1920, 1024, null},
            new object[]{1920, 1024, null},
            new object[]{1920, 1024, null},
            new object[]{1920, 1024, null}
        };

        [Test, TestCaseSource(nameof(_testCases))]
        public void CloudLayoutIsShrinking_WithCompactRequired(int centerPointX, int centerPointY, List<Size> sizeList)
        {
            sizeList = sizeList ?? GenerateRectangleSizes(250).ToList();
            ParametrizeLayouter(centerPointX, centerPointY, sizeList);
            layouter.CompactRequired = false;
            DoLayout();
            var circumcircleArea = CalculateСircumcircleArea(layouter.GetCurrentLayout().ToList());
            SaveResults();

            ParametrizeLayouter(centerPointX, centerPointY, sizeList);
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

        [TestCase(1920, 1024)]
        public void CloudLayouterIsPlacingRectangles(int centerPointX, int centerPointY)
        {
            ParametrizeLayouter(centerPointX, centerPointY, new []{new Size(50,70),new Size(50,70),new Size(50,70), new Size(50, 70), new Size(50, 70) });
            DoLayout();
            placedRectangles.Count(r => placedRectangles.Count(r2 =>
                                            r.Left == r2.Left && r.Right == r2.Right && r.Top == r2.Top &&
                                            r.Bottom == r2.Bottom) > 1).Should().Be(0);
        }

        [TestCase(1920, 1024, 50, 30, 250, 80, 120, ExpectedResult = false)]
        [TestCase(1920, 1024, 20, 20, 20, 20, 250, ExpectedResult = false)]
        public bool RectanglesHaveNoIntersects_Always(int centerPointX, int centerPointY, int minRectangleSizeWidth, int minRectangleSizeHeight, int maxRectangleSizeWidth, int maxRectangleSizeHeight, int rectanglesCount)
        {
            ParametrizeLayouter(centerPointX, centerPointY, null, rectanglesCount, minRectangleSizeWidth, minRectangleSizeHeight, maxRectangleSizeWidth, maxRectangleSizeHeight);
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
        [TestCase(1920, 1024, 20, 20, 500, 70, 0.6, 500, 4, 0.314, true, ExpectedResult = true)]
        public bool TagCloudIsCompactEnogth_DependsOnCompactRequired(int centerPointX, int centerPointY, int minRectangleSizeWidth, int minRectangleSizeHeight, int maxRectangleSizeWidth, int maxRectangleSizeHeight, double compactRelation, int rectanglesCount, int spiralStep = 15, double spiralAngleStep = 0.314, bool doCompact = true)
        {
            ParametrizeLayouter(centerPointX, centerPointY, null, rectanglesCount, minRectangleSizeWidth, minRectangleSizeHeight, maxRectangleSizeWidth, maxRectangleSizeHeight);
            layouter.CompactRequired = doCompact;
            layouter.SpiralStepSize = spiralStep;
            layouter.SpiralStepAngleRadians = spiralAngleStep;
            DoLayout();
            var rectanglesCoveredArea = CalculateRectanglesArea(placedRectangles);
            var circumcircleArea = CalculateСircumcircleArea(placedRectangles);
            CoverageReport(rectanglesCoveredArea, circumcircleArea);
            return (rectanglesCoveredArea / circumcircleArea) > (compactRelation);
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
            presenter.PresentCloudToFile(layouter, fileName);

        }

        private static double CalculateRectanglesArea(IEnumerable<Rectangle> rectangles)
        {
            return rectangles.Select(CalculateRectangleArea).Sum();
        }

        private static int CalculateRectangleArea(Rectangle rectangle)
        {
            return rectangle.Height * rectangle.Width;
        }

        private double CalculateСircumcircleArea(IEnumerable<Rectangle> rectangles)
        {
            var сircumcircleRadius = CalculateСircumcircleRadius(rectangles);
            return сircumcircleRadius * сircumcircleRadius * Math.PI;
        }

        private double CalculateСircumcircleRadius(IEnumerable<Rectangle> rectangles)
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