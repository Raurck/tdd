using System;
using System.Drawing;
using System.Linq;
using Grace.DependencyInjection;
using TagCloudLayouter;
using TagCloudLayouter.Interfaces;


namespace TagColudConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var fr = new SimpleTextFileReader();
            var str = fr.Read("D:\\test.txt");
            var pr = new SimpleWordPreprocessor(new SimpleWordTransformer());
            var res =  pr.ProcessStrings(str);
            //var dc = from pair in res
            //          orderby pair.Value descending 
            //select pair;
            var dc = res.OrderByDescending(g => g.Value).Take(100);
            var tfp = new TagCloudFontProvider(dc, 10, 50);
            var s = new WordRectangleGenerator();
            var layouter = new CircularCloudLayouter(new Point(1920, 1024/2));
            var tpls =  dc.Select(g =>
                new Tuple<Rectangle, Font, string>(layouter.PutNextRectangle(s.GetWordBoundingSize(g.Key, tfp.GetFontForFrequency(g.Value))),
                    tfp.GetFontForFrequency(g.Value), g.Key));
            var tagCloudPresenter = new TagCloudPresenter(1920*2,1280*2);
            tagCloudPresenter.PresentCloudToFile(tpls, "D:\\1.png");
        }
    }
}
