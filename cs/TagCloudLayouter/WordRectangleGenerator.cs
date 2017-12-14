using System;
using System.Drawing;
using TagCloud.Interfaces;


namespace TagCloud
{
    public class WordRectangleGenerator : IWordRectangleGenerator
    {
        public Size GetWordBoundingSize(string word, Font font)
        {
            using (var canvas = Graphics.FromHwnd(IntPtr.Zero))
            {
                return Size.Ceiling(canvas.MeasureString(word, font, new PointF(0, 0), StringFormat.GenericDefault));
            }
        }


    }


}