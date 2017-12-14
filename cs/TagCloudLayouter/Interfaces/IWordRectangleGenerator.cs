using System.Drawing;

namespace TagCloud.Interfaces
{
    public interface IWordRectangleGenerator
    {
        Size GetWordBoundingSize(string word, Font font);
    }
}