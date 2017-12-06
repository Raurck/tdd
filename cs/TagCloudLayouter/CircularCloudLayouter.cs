using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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
        private List<Rectangle> placedRectangles = new List<Rectangle>();
        private Random RandomGenerator = new Random();
        private int circleRadius = 0;

        public Rectangle PutNextRectangle(Size rectangleSize)
        {
            var resultRectangle = new Rectangle(cloudCenter, rectangleSize);
            FindRectaglePlace(resultRectangle);
            placedRectangles.Add(resultRectangle);
            return resultRectangle;
        }

        private void FindRectaglePlace(Rectangle currentRectangle)
        {
            currentRectangle.X = RandomGenerator.Next(2 * circleRadius) - cloudCenter.X;
            currentRectangle.Y = RandomGenerator.Next(2 * circleRadius) - cloudCenter.Y;
        }

        private void HasIntersectWithOthers(Rectangle currentRectangle)
        {
            throw new NotImplementedException();
        }

        private void HasIntersectWithOne(Rectangle currentRectangle, Rectangle checkedRectangle)
        {
            throw new NotImplementedException();
        }

        public Bitmap Draw()
        {
            var result = new Bitmap(1920, 1024);
            using (var canvas = Graphics.FromImage(result))
            {
                //canvas.DrawEllipse(new Pen(Color.Blue), 20, 300, 50, 200);
                //canvas.FillRectangles(new SolidBrush(Color.Blue), placedRectangles.ToArray());
                canvas.CopyFromScreen(0, 0, 1920, 1280, new Size(1920, 1080));
            }
            //            Canvas.DrawRectangles(new Pen(Color.Blue, 1), placedRectangles.ToArray());
            return result;
        }

        public void Save(string fileName)
        {
            if (String.IsNullOrWhiteSpace(fileName))
            {
                fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "results", "rectangles.png");
            }
            var resultDirectory = Path.GetDirectoryName(fileName);
            if (!Directory.Exists(resultDirectory))
            {
                Directory.CreateDirectory(resultDirectory);
            }
            using (var bitmap = Draw())
            {
                bitmap?.Save(fileName, ImageFormat.Png);
            }
        }

        public CircularCloudLayouter(Point center)
        {
            cloudCenter = center;
        }
    }

    [TestFixture]
    class CircularCloudLayouterSould
    {
        private CircularCloudLayouter layouter;
        private List<Rectangle> placedRectangles;


        private readonly int _testPoolSize = 200;
        private IEnumerable<Size> GetRectangleSizes(int rectangleCount)
        {
            var Rectangles = new List<Size>();
            var RandomGenerator = new Random();
            for (var i = 0; i < rectangleCount; i++)
            {
                Rectangles.Add(new Size(RandomGenerator.Next(250), RandomGenerator.Next(80)));
            }
            return Rectangles;
        }

        [SetUp]
        public void SetUp()
        {
            layouter = new CircularCloudLayouter(new Point(950, 512));
            var sizedRects = GetRectangleSizes(_testPoolSize);
            placedRectangles = new List<Rectangle>();
            foreach (var sizedRect in sizedRects)
            {
                placedRectangles.Add(layouter.PutNextRectangle(sizedRect));
            }
        }

        [Test]
        public void RectanglesHaveNoIntersects()
        {
            Assert.AreEqual(0, 1);
        }

        [TearDown]
        public void SaveResultOnFail()
        {
            //if (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed)
            {
                layouter.Save(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "results",
                    $"{TestContext.CurrentContext.Test.MethodName}.png"));
            }
        }
    }
}