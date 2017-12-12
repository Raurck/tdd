using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Microsoft.VisualStudio.Imaging;

namespace TagCloudLayouter
{
    public class TagCloudFontProvider
    {
        private readonly int maxWordCount;
        private readonly int minWordCount;
        private readonly int minFontSize;
        private readonly int maxFontSize;

        private IEnumerable<string> GetFontName()
        {
            do
            {
                yield return "Arial";
                yield return "Calibri";
                yield return "Times New Roman";
                yield return "Courier";
            } while(true);
        }

        public Font GetFontForFrequency(int frequency)
        {
            var fontSize = 1f;
            if (frequency <= minWordCount) { fontSize = minFontSize;}
            if (frequency >= maxWordCount) { fontSize = maxFontSize; }
            if (frequency < maxWordCount && frequency > minWordCount)
            {
                fontSize =
                    ((float)(frequency - minWordCount) / (maxWordCount - minWordCount))*(maxFontSize - minFontSize) + minFontSize;
            }
            return new Font(GetFontName().ToString(), fontSize);
        }

        public TagCloudFontProvider(IEnumerable<KeyValuePair<string,int>> words, int minFontSize, int maxFontSize)
        {
            if (words == null)
            {
                throw new ArgumentNullException();
            }
            if (!words.ToArray().Any())
            {
                throw new ArgumentException();
            }
            if (minFontSize >= maxFontSize)
            {
                throw new ArgumentOutOfRangeException();
            }

            maxWordCount = words.Select(group=>group.Value).Max();
            minWordCount = words.Select(group => group.Value).Min();
            this.minFontSize = minFontSize;
            this.maxFontSize = maxFontSize;
        }
    }
}