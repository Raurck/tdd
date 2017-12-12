using System.Drawing;
using System.Windows.Forms;
using FluentAssertions.Common;
using TagCloudLayouter.Interfaces;


namespace TagCloudLayouter
{
    public class WordRectangleGenerator: IWordRectangleGenerator
    {
        public Size GetWordBoundingSize(string word, Font font)
        {
            return TextRenderer.MeasureText(word.Capitalize(), font);
        }
    }
}