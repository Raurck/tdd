using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using TagCloudLayouter.Interfaces;

namespace TagCloudLayouter
{
    public class TagCloudPresenter
    {
        protected int canvasWidth;
        protected int canvasHeight;
        public string DefaultSubdirectory { get; set; } = "results";
        public string DefaultFileName { get; set; } = "rectangles.png";


        public TagCloudPresenter(int canvasWidth, int canvasHeight)
        {
            this.canvasWidth = canvasWidth;
            this.canvasHeight = canvasHeight;
        }

        protected void DrawRectangles(Graphics canvas, IEnumerable<Tuple<Rectangle, Font, string>> tagTuples)
        {
            var tagTuplesArray = tagTuples.ToArray();
            var rectangleColorProvider = new RectangleColorProvider(tagTuplesArray.Select(tpl => tpl.Item1));
            foreach (var tagTuple in tagTuplesArray)
            {
                canvas.DrawString(tagTuple.Item3, tagTuple.Item2, new SolidBrush(rectangleColorProvider.GetRectangleColor(tagTuple.Item1)), tagTuple.Item1);
                //   canvas.DrawRectangle(new Pen(rectangleColorProvider.GetRectangleColor(placedRectangle), 2), placedRectangle);
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

        public virtual void PresentCloudToFile(IEnumerable<Tuple<Rectangle, Font, string>> tagObjects, string fileName)
        {
            fileName = GetRealFileName(fileName);
            using (var presentation = new Bitmap(canvasWidth, canvasHeight))
            {
                using (var canvas = Graphics.FromImage(presentation))
                {
                    DrawRectangles(canvas, tagObjects);
                }
                presentation?.Save(fileName);
            }
        }
    }
}