using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Microsoft.VisualStudio.Imaging;

namespace TagCloud
{
    public class RectangleColorProvider
    {
        private readonly Dictionary<int, float> rectanglesColorDictonary;

        private readonly int maxRectangleHeight;
        private readonly int minRectangleHeight;
        public int ColorHue { get; set; } = 240;
        public double ColorLuminosity { get; set; } = 0.5;

        public Color GetRectangleColor(Rectangle rectangle)
        {
            if (rectangle == null)
            {
                throw new ArgumentNullException();
            }

            var saturation = GetSaturationByHeight(rectangle.Height);
            var hslColor = new HslColor(ColorHue, saturation, ColorLuminosity,1);
            var mediaColor = hslColor.ToColor();
            return  Color.FromArgb(mediaColor.A, mediaColor.R, mediaColor.G, mediaColor.B); 
        }

        private double GetSaturationByHeight(int height)
        {
            var saturation = 0f;
            if (height <= minRectangleHeight) return 0;
            if (height >= maxRectangleHeight) return 1;

            bool fontSizeIsSet;              
            do
            {
                fontSizeIsSet = rectanglesColorDictonary.TryGetValue(height, out saturation);
                height--;
            } while (!fontSizeIsSet && height >= minRectangleHeight);

            if (!fontSizeIsSet)
            {
                saturation = 0;
            }
            return saturation;
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

            var rectanglesHeightArray = finishedList.Select(rectangle => rectangle.Height)
                                            .Distinct()
                                            .OrderBy(x => x)
                                            .ToArray();
            var rectanglesColorDictonaryLength = rectanglesHeightArray.Count();
            rectanglesColorDictonary = rectanglesHeightArray
                .Select((height, pos) => new KeyValuePair<int, float>(height, 1f/5 + 4f/5* (float)pos / rectanglesColorDictonaryLength))
                .ToDictionary(kv => kv.Key, kv => kv.Value);
        }

    }
}