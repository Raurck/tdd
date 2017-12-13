using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace TagCloudLayouter
{
    public class TagCloudFontProvider
    {
        private readonly Dictionary<int, float> fontSizeDictonary;
        private readonly int minWordCount;
        private readonly int maxWordCount;
        private readonly int minFontSize;
        private readonly int maxFontSize;

        private readonly string[] fontNames =
        {
            "Arial",
            "Calibri",
            "Times New Roman",
            "Courier"
        };

        public TagCloudFontProvider(IEnumerable<KeyValuePair<string, int>> words, int minFontSize, int maxFontSize)
        {
            if (words == null)
            {
                throw new ArgumentNullException();
            }

            words = words.ToArray();
            if (!words.Any())
            {
                throw new ArgumentException();
            }

            if (minFontSize >= maxFontSize)
            {
                throw new ArgumentOutOfRangeException();
            }

            var wordCounters = words.Select(group => group.Value).Distinct().OrderBy(w => w).ToArray();
            var fontDictonatyLength = wordCounters.Count();
            fontSizeDictonary = wordCounters
                .Select((count, pos) => new KeyValuePair<int, float>(count, minFontSize + ((float)maxFontSize - minFontSize) * pos / fontDictonatyLength))
                .ToDictionary(kv => kv.Key, kv => kv.Value);

            maxWordCount = words.Select(group => group.Value).Max();
            minWordCount = words.Select(group => group.Value).Min();
            this.minFontSize = minFontSize;
            this.maxFontSize = maxFontSize;
        }


        private string GetFontName(int index)
        {
            return fontNames[index % fontNames.Length];
        }

        public Font GetFontForFrequency(int frequency)
        {
            var fontSize = 1f;
            if (frequency <= minWordCount) { return new Font(GetFontName(frequency), minFontSize); }
            if (frequency >= maxWordCount) { return new Font(GetFontName(frequency), maxFontSize); }

            bool fontSizeIsSet;
            do
            {
                fontSizeIsSet = fontSizeDictonary.TryGetValue(frequency, out fontSize);
                frequency--;
            } while (!fontSizeIsSet && frequency >= minWordCount);

            if (!fontSizeIsSet)
            {
                fontSize = minFontSize;
            }
            return new Font(GetFontName(frequency), (float)Math.Ceiling(fontSize));
        }

       
    }
}