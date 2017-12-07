using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using TagCloudLayouter.Interfaces;

namespace TagCloudLayouter
{
    public class CloudPresenter
    {
        protected int canvasWidth;
        protected int canvasHeight;
        public string DefaultSubdirectory { get; set; } = "results";
        public string DefaultFileName { get; set; } = "rectangles.png";


        public CloudPresenter(int canvasWidth, int canvasHeight)
        {
            this.canvasWidth = canvasWidth;
            this.canvasHeight = canvasHeight;
        }

        protected void DrawRectangles(Graphics canvas, IEnumerable<Rectangle> rectangles)
        {
            var placedRectangles = rectangles.ToArray();
            var rectangleColorProvider = new RectangleColorProvider(placedRectangles);
            foreach (var placedRectangle in placedRectangles)
            {
                canvas.DrawRectangle(new Pen(rectangleColorProvider.GetRectangleColor(placedRectangle), 2), placedRectangle);
            }
        }

        protected string GetRealFileName(string fileName)
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

            if (File.Exists(fileName))
            {
                var namePart = Path.GetFileNameWithoutExtension(fileName);
                var extensionPart = Path.GetExtension(fileName);
                fileName = Path.Combine(resultDirectory, $"{namePart}_{DateTime.Now.Millisecond}{extensionPart}");
            }

            return fileName;
        }

        public virtual void PresentCloudToFile(ICloudLayouter layouter, string fileName)
        {
            fileName = GetRealFileName(fileName);
            using (var presentation = new Bitmap(canvasWidth, canvasHeight))
            {
                using (var canvas = Graphics.FromImage(presentation))
                {
                    DrawRectangles(canvas, layouter.GetCurrentLayout());
                }
                presentation?.Save(fileName);
            }
        }

    }
}