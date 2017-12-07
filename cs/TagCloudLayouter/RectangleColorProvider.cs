using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Microsoft.VisualStudio.Imaging;

namespace TagCloudLayouter
{
    public class RectangleColorProvider
    {
        private readonly int maxRectangleHeight;
        private readonly int minRectangleHeight;
        public int ColorHue { get; set; } = 240;
        public double ColorLuminosity { get; set; } = 0.5;

        public Color GetRectangleColor(Rectangle rectangle)
        {
            var saturation = GetSaturationByHeight(rectangle);
            var hslColor = new HslColor(ColorHue, saturation, ColorLuminosity,1);
            var mediaColor = hslColor.ToColor();
            return  Color.FromArgb(mediaColor.A, mediaColor.R, mediaColor.G, mediaColor.B); 
        }

        private double GetSaturationByHeight(Rectangle rectangle)
        {
            if (maxRectangleHeight - minRectangleHeight == 0)
            {
                return 1;
            }
            return 1 - (((double)(maxRectangleHeight - rectangle.Height))/(maxRectangleHeight - minRectangleHeight))*0.7;
        }

        public RectangleColorProvider(IEnumerable<Rectangle> rectangles)
        {
            if (rectangles == null)
            {
                throw new ArgumentNullException();
            }
            var finishedList = rectangles.ToArray();
            maxRectangleHeight = finishedList.Select(rect => rect.Height).Max();
            minRectangleHeight = finishedList.Select(rect => rect.Height).Min();
        }

    }
}