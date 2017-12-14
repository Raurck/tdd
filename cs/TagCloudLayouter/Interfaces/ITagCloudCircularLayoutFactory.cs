using System.Drawing;

namespace TagCloud.Interfaces
{
    public interface ITagCloudCircularLayoutFactory
    {
        ICloudLayouter CreateCircularCloudLayouter(Point center);
    }
}