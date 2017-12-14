namespace TagCloud.Interfaces
{
    public interface ICloudPresenterFactory
    {
        ICloudPresenter CreateTagCloudPresenter(int canvasWidth, int canvasHeight, RectangleColorProvider rectangleColorProvider);
    }
}