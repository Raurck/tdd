using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;

namespace TagCloudLayouter
{
    public class TagCloudPresenter
    {
        protected int CanvasWidth;
        protected int CanvasHeight;
        public string DefaultSubdirectory { get; set; } = "results";
        public string DefaultFileName { get; set; } = "rectangles.png";


        public TagCloudPresenter(int canvasWidth, int canvasHeight)
        {
            CanvasWidth = canvasWidth;
            CanvasHeight = canvasHeight;
        }

        protected void DrawRectangles(Graphics canvas, IEnumerable<Tuple<Rectangle, Font, string>> tagTuples)
        {
            var tagTuplesArray = tagTuples.ToArray();
            var rectangleColorProvider = new RectangleColorProvider(tagTuplesArray.Select(tpl => tpl.Item1));
            foreach (var tagTuple in tagTuplesArray)
            {
                //canvas.DrawRectangle(new Pen(rectangleColorProvider.GetRectangleColor(tagTuple.Item1), 2), tagTuple.Item1);
                canvas.DrawString(tagTuple.Item3, tagTuple.Item2,
                    new SolidBrush(rectangleColorProvider.GetRectangleColor(tagTuple.Item1)), tagTuple.Item1,
                    StringFormat.GenericDefault);
                  
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

        private void DrawOnCanvas(Bitmap bitmap, IEnumerable<Tuple<Rectangle, Font, string>> tagObjects)
        {
            using (var canvas = Graphics.FromImage(bitmap))
            {
                canvas.TextRenderingHint = TextRenderingHint.AntiAlias;
                DrawRectangles(canvas, tagObjects);
            }
        }

        public virtual void PresentCloudToFile(IEnumerable<Tuple<Rectangle, Font, string>> tagObjects, string fileName)
        {
            fileName = GetRealFileName(fileName);
            using (var presentation = new Bitmap(CanvasWidth, CanvasHeight))
            {
                DrawOnCanvas(presentation, tagObjects);
                presentation.Save(fileName);
            }
        }

        public virtual Bitmap PresentCloudAsBitmap(IEnumerable<Tuple<Rectangle, Font, string>> tagObjects)
        {
            using (var presentation = new Bitmap(CanvasWidth, CanvasHeight))
            {
                DrawOnCanvas(presentation, tagObjects);
                return presentation;
            }
        }
    }
}