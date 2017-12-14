using System;
using System.Drawing;
using System.IO;
using System.Linq;
using Grace.DependencyInjection;
using TagCloud;
using TagCloud.Interfaces;


namespace TagColudConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var tagColud = new SimpleTagCloud();
            tagColud.GenerateTagCloudFromTextFile(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test_eng.txt"),
                100).Save("D:\\1.png");
            /*
            var fr = new SimpleTextFileReader();
            var str = fr.Read(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test_eng.txt"));
            var boringWordsList = fr.Read(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"boring.txt"));
            var pr = new SimplePreprocessor(new SimpleWordTransformer(boringWordsList), 100);
            var res =  pr.ProcessStrings(str).ToArray();
            var tfp = new TagCloudFontProvider(res, 10, 50);
            var s = new WordRectangleGenerator();
            var layouter = new CircularCloudLayouter(new Point(1920/2, 1024 / 2));
            var tpls = res.Select(g =>
                new Tuple<Rectangle, Font, string>(layouter.PutNextRectangle(s.GetWordBoundingSize(g.Key, tfp.GetFontForFrequency(g.Value))),
                    tfp.GetFontForFrequency(g.Value), g.Key));
            var tagCloudPresenter = new TagCloudPresenter(1920,1024);
            tagCloudPresenter.PresentCloudToFile(tpls, "D:\\1.png");*/
        }
    }
}
