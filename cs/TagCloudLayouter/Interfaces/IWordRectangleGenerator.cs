using System.Drawing;

namespace TagCloudLayouter.Interfaces
{
    public interface IWordRectangleGenerator
    {
        Size GetWordBoundingSize(string word, Font font);
    }
}