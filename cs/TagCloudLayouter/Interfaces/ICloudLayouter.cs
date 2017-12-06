using System.Drawing;

namespace TagCloudLayouter.Interfaces
{
    interface ICloudLayouter
    {
        Rectangle PutNextRectangle(Size rectangleSize);
    }
}
