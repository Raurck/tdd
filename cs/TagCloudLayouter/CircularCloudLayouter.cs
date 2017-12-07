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

		private readonly Point cloudCenter;
		private readonly List<Rectangle> placedRectangles = new List<Rectangle>();

		public bool CompactRequired { get; set; } = true;
		public int CanvasWidth { get; set; } = 1920;
		public int CanvasHeight { get; set; } = 1024;
		public double SpiralStepAngleRadians { get; set; } = Math.PI / 25;
		public int SpiralStepSize { get; set; } = 1;
		public string DefaultSubdirectory { get; set; } = "results";
		public string DefaultFileName { get; set; } = "rectangles.png";

		public Rectangle PutNextRectangle(Size rectangleSize)
		{
			var resultRectangle = FindRectaglePlace(rectangleSize);
			if (CompactRequired)
			{
				resultRectangle = CompactRegion(resultRectangle);
			}
			placedRectangles.Add(resultRectangle);
			return resultRectangle;
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

		protected virtual Rectangle CompactRegion(Rectangle currentRectangle)
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

		protected Bitmap Draw()
		{
			var rectangleColorProvider = new RectangleColorProvider(placedRectangles);
			var result = new Bitmap(CanvasWidth, CanvasHeight);
			using (var canvas = Graphics.FromImage(result))
			{
				foreach (var placedRectangle in placedRectangles)
				{
					canvas.DrawRectangle(new Pen(rectangleColorProvider.GetRectangleColor(placedRectangle), 2), placedRectangle);
				}
			}
			return result;
		}

		public virtual void Save(string fileName)
		{
			if (String.IsNullOrWhiteSpace(fileName))
			{
				fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DefaultSubdirectory, DefaultFileName);
			}

			var resultDirectory = Path.GetDirectoryName(fileName);
			if (String.IsNullOrWhiteSpace(resultDirectory))
			{
				resultDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DefaultSubdirectory);
				fileName = Path.Combine(resultDirectory, DefaultFileName);
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
	internal class CircularCloudLayouter_Should
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