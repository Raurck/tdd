using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace TagCloud.Interfaces
{
    public interface ICloudLayouter
    {
        Rectangle PutNextRectangle(Size rectangleSize);
        IEnumerable<Rectangle> GetCurrentLayout();
        Point GetCloudCenter();
    }
}
