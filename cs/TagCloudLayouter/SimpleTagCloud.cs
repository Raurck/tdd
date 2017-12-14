using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Grace.DependencyInjection;
using Grace.Factory;
using TagCloud.Interfaces;

namespace TagCloud
{
    public class SimpleTagCloud
    {
        private readonly ITextFileReader fileReader;
        private readonly ICloudPresenterFactory cloudPresenterFactory;
        private readonly IWordRectangleGenerator rectangleGenerator;
        private readonly ISimpleTextPreprocessorFactory preprocessorFactory;
        private readonly ITagCloudFontProviderFactory fontFactory;
        private readonly ITagCloudCircularLayoutFactory layoutFactory;
        private readonly IRectangleColorProviderFactory rectangleColorProviderFactory;

        public int MinFontSize { get; set; } = 10;
        public int MaxFontSize { get; set; } = 50;
        public Point CloudCenter { get; set; } = new Point(1920 / 2, 1200 / 2);
        public Size CanvasSize { get; set; } = new Size(1920, 1200);


        public SimpleTagCloud()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<CircularCloudLayouter>().As<ICloudLayouter>();
                c.Export<SimpleWordTransformer>().As<IWordTransformer>();
                c.Export<WordRectangleGenerator>().As<IWordRectangleGenerator>();
                c.Export<SimpleTextFileReader>().As<ITextFileReader>();
                c.Export<SimplePreprocessor>().As<ITagCloudTextPreprocessor>();
                c.Export<TagCloudPresenter>().As<ICloudPresenter>();
                c.ExportFactory<ISimpleTextPreprocessorFactory>();
                c.ExportFactory<ITagCloudFontProviderFactory>();
                c.ExportFactory<ITagCloudCircularLayoutFactory>();
                c.ExportFactory<IRectangleColorProviderFactory>();
                c.ExportFactory<ICloudPresenterFactory>();
            });
            fileReader = container.Locate<ITextFileReader>();
            rectangleGenerator = container.Locate<IWordRectangleGenerator>();
            preprocessorFactory = container.Locate<ISimpleTextPreprocessorFactory>();
            fontFactory = container.Locate<ITagCloudFontProviderFactory>();
            layoutFactory = container.Locate<ITagCloudCircularLayoutFactory>();
            rectangleColorProviderFactory = container.Locate<IRectangleColorProviderFactory>();
            cloudPresenterFactory = container.Locate<ICloudPresenterFactory>();
        }

        public Bitmap GenerateTagCloudFromTextFile(
            string fileName,
            int wordsInCloud,
            IEnumerable<char> wordSplitters = null,
            IEnumerable<string> boringWords = null)
        {
            var preprocessor = preprocessorFactory.CreateSimplePreprocessor(wordsInCloud, wordSplitters);
            var tagSourceText = fileReader.Read(fileName);
            var preparedWords = preprocessor.ProcessStrings(tagSourceText).ToArray();
            var fontProvider = fontFactory.CreateTagCloudFontProvider(preparedWords, MinFontSize, MaxFontSize);
            var tagLayouter = layoutFactory.CreateCircularCloudLayouter(CloudCenter);
            var presenterData = preparedWords.Select(wordFrequencyGroup =>
                new Tuple<Rectangle, Font, string>(tagLayouter.PutNextRectangle(rectangleGenerator.GetWordBoundingSize(wordFrequencyGroup.Key, fontProvider.GetFontForFrequency(wordFrequencyGroup.Value))),
                    fontProvider.GetFontForFrequency(wordFrequencyGroup.Value), wordFrequencyGroup.Key)).ToArray();
            var colorFactory = rectangleColorProviderFactory.CreateRectangleColorProvider(presenterData.Select(data => data.Item1));
            var cloudPresenter = cloudPresenterFactory.CreateTagCloudPresenter(CanvasSize.Width, CanvasSize.Height, colorFactory);
            return cloudPresenter.PresentCloudAsBitmap(presenterData);
        }
    }
}