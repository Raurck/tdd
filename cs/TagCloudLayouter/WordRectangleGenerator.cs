using System;
using System.Drawing;
using FluentAssertions.Common;
using System.Windows.Forms;
using TagCloudLayouter.Interfaces;


namespace TagCloudLayouter
{
    public class WordRectangleGenerator: IWordRectangleGenerator
    {
        public Size GetWordBoundingSize(string word, Font font)
        {
            using (var canvas = Graphics.FromHwnd(IntPtr.Zero))
                return Size.Round(canvas.MeasureString(word, font));
        }
    }
}