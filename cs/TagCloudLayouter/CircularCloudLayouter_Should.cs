using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
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
		private const int TestPoolSize = 150;
		private const int CloudCenterX = 1920 / 2;
		private const int CloudCenterY = 1024 / 2;
		private const int MaxWitdh = 250;
		private const int MaxHeight = 60;
		private const int MinWitdh = 50;
		private const int MinHeight = 30;
		private const double CompactFactor = 0.5;

		private static IEnumerable<Size> GenerateRectangleSizes(int rectangleCount)
		{
			var rectangles = new List<Size>();
			var randomGenerator = new Random();
			for (var i = 0; i < rectangleCount; i++)
			{
				rectangles.Add(new Size(MinWitdh + randomGenerator.Next(MaxWitdh - MinWitdh), MinHeight + randomGenerator.Next(MaxHeight - MinHeight)));
			}
			return rectangles;
		}

		[SetUp]
		public void SetUp()
		{
			layouter = new CircularCloudLayouter(new Point(CloudCenterX, CloudCenterY));
			sizedRects = GenerateRectangleSizes(TestPoolSize);
			placedRectangles = new List<Rectangle>();
		}

		[Test]
		public void RectanglesHaveNoIntersects_Always()
		{
			DoLayout();
			var hasIntersects = placedRectangles.Any(HasIntersectWithOthers);
			Assert.AreEqual(false, hasIntersects, "Rectangles have intersects with each others");
		}

		[Test]
		public void CompactEnougth_ByDefault()
		{
			DoLayout();
			var rectanglesCoveredArea = CalculateRectanglesArea(placedRectangles);
			var circumcircleArea = CalculateСircumcircleArea(placedRectangles);
			CoverageReport(rectanglesCoveredArea, circumcircleArea);
			Assert.GreaterOrEqual(rectanglesCoveredArea / circumcircleArea, CompactFactor, "The cloud is not compact enogth");
		}

		[Test]
		public void NotSoCompact_WithCompactDisabled()
		{
			layouter.CompactRequired = false;
			layouter.SpiralStepSize = 15;
			DoLayout();
			var rectanglesCoveredArea = CalculateRectanglesArea(placedRectangles);
			var circumcircleArea = CalculateСircumcircleArea(placedRectangles);
			CoverageReport(rectanglesCoveredArea, circumcircleArea);
			Assert.LessOrEqual(rectanglesCoveredArea / circumcircleArea, CompactFactor, "The cloud is too compact enogth");
		}

		[TearDown]
		public void TearDown()
		{
			SaveResults();
		}


		private void CoverageReport(double rectanglesCoveredArea, double circumcircleArea)
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

			layouter.Save(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, subDirectory,
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
			return placedRectangles.Where(placedRectangle => placedRectangle != currentRectangle)
				.Any(placedRectangle => HasIntersectWithOne(currentRectangle, placedRectangle));
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