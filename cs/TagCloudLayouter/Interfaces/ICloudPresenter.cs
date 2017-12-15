using System;
using System.Collections.Generic;
using System.Drawing;

namespace TagCloud.Interfaces
{
    public interface ICloudPresenter
    {
        string PresentCloudToFile(IEnumerable<Tuple<Rectangle, Font, string>> tagObjects, string fileName);
        Bitmap PresentCloudAsBitmap(IEnumerable<Tuple<Rectangle, Font, string>> tagObjects);
    }
}