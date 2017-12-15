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
        //private readonly IWordTransformerFactory wordTransformer;
        private readonly ITagCloudFontProviderFactory fontFactory;
        private readonly ITagCloudCircularLayoutFactory layoutFactory;
        private readonly IRectangleColorProviderFactory rectangleColorProviderFactory;
        private IEnumerable<string> boringWords;
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
                c.Export<SimpleWordTransformer>().As<IWordTransformer>().WithCtorParam(()=>boringWords);
                c.Export<WordRectangleGenerator>().As<IWordRectangleGenerator>();
                c.Export<SimpleTextFileReader>().As<ITextFileReader>();
                c.Export<SimplePreprocessor>().As<ITagCloudTextPreprocessor>();
                c.Export<TagCloudPresenter>().As<ICloudPresenter>();
                c.ExportFactory<ISimpleTextPreprocessorFactory>();
                c.ExportFactory<ITagCloudFontProviderFactory>();
                c.ExportFactory<ITagCloudCircularLayoutFactory>();
                c.ExportFactory<IRectangleColorProviderFactory>();
                c.ExportFactory<ICloudPresenterFactory>();
                //c.ExportFactory<IWordTransformerFactory>();
            });
            fileReader = container.Locate<ITextFileReader>();
            rectangleGenerator = container.Locate<IWordRectangleGenerator>();
            preprocessorFactory = container.Locate<ISimpleTextPreprocessorFactory>();
            fontFactory = container.Locate<ITagCloudFontProviderFactory>();
            layoutFactory = container.Locate<ITagCloudCircularLayoutFactory>();
            rectangleColorProviderFactory = container.Locate<IRectangleColorProviderFactory>();
            cloudPresenterFactory = container.Locate<ICloudPresenterFactory>();
            //wordTransformer = container.Locate<IWordTransformerFactory>();

        }


        private IEnumerable<Tuple<Rectangle, Font, string>> GetPrimitiveTupples(string fileName,
            int wordsInCloud,
            IEnumerable<string> boringWordsEnum,
            IEnumerable<char> wordSplitters
            )
        {
            boringWords = boringWordsEnum;
            var preprocessor = preprocessorFactory.CreateSimplePreprocessor(wordsInCloud, wordSplitters);
            var tagSourceText = fileReader.Read(fileName);
            var preparedWords = preprocessor.ProcessStrings(tagSourceText).ToArray();
            var fontProvider = fontFactory.CreateTagCloudFontProvider(preparedWords, MinFontSize, MaxFontSize);
            var tagLayouter = layoutFactory.CreateCircularCloudLayouter(CloudCenter);
            return preparedWords.Select(wordFrequencyGroup =>
                new Tuple<Rectangle, Font, string>(tagLayouter.PutNextRectangle(rectangleGenerator.GetWordBoundingSize(wordFrequencyGroup.Key, fontProvider.GetFontForFrequency(wordFrequencyGroup.Value))),
                    fontProvider.GetFontForFrequency(wordFrequencyGroup.Value), wordFrequencyGroup.Key));
        }

        public Bitmap GenerateTagCloudFromTextFile(
            string fileName,
            int wordsInCloud,
            IEnumerable<string> boringWords = null,
            IEnumerable<char> wordSplitters = null)
        {
            var presenterData = GetPrimitiveTupples(fileName, wordsInCloud, boringWords, wordSplitters).ToArray();
            var colorFactory = rectangleColorProviderFactory.CreateRectangleColorProvider(presenterData.Select(data => data.Item1));
            var cloudPresenter = cloudPresenterFactory.CreateTagCloudPresenter(CanvasSize.Width, CanvasSize.Height, colorFactory);
            return cloudPresenter.PresentCloudAsBitmap(presenterData);
        }

        public string GenerateImageFileFromTextFile(
            string fileName,
            int wordsInCloud,
            string expectedFileName,
            IEnumerable<string> boringWords = null,
            IEnumerable<char> wordSplitters = null)
        {
            var presenterData = GetPrimitiveTupples(fileName, wordsInCloud, boringWords, wordSplitters).ToArray();
            var colorFactory = rectangleColorProviderFactory.CreateRectangleColorProvider(presenterData.Select(data => data.Item1));
            var cloudPresenter = cloudPresenterFactory.CreateTagCloudPresenter(CanvasSize.Width, CanvasSize.Height, colorFactory);
            return cloudPresenter.PresentCloudToFile(presenterData, expectedFileName);
        }
    }
}