using System.Collections.Generic;
using System.Drawing;

namespace TagCloud.Interfaces
{
    public interface IRectangleColorProviderFactory
    {
        RectangleColorProvider CreateRectangleColorProvider(IEnumerable<Rectangle> rectangles);
    }
}