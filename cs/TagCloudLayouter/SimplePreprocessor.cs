using System.Collections.Generic;
using System.Linq;
using TagCloud.Interfaces;

namespace TagCloud
{
    public class SimplePreprocessor : ITagCloudTextPreprocessor
    {
        private readonly IWordTransformer wordTransformer;
        private readonly int tagCloudWordAmount;
        private IEnumerable<char> wordSplitters = new[] {' ', ',', '.', ';', ':', '\'', '-', '\n', ' ', '’', '”', '“', '—' };

        public SimplePreprocessor(
            IWordTransformer wordTransformer,
            int tagCloudWordAmount,
            IEnumerable<char> wordSplitters = null)
        {
            this.wordTransformer = wordTransformer;
            this.tagCloudWordAmount = tagCloudWordAmount;
            this.wordSplitters = wordSplitters ?? this.wordSplitters;
        }

        public IEnumerable<KeyValuePair<string, int>> ProcessStrings(string[] text)
        {
            return text
                     .SelectMany(str => str.Split(wordSplitters.ToArray()))
                     .Select(w => wordTransformer.GetTransformedWord(w))
                     .Where(w => !string.IsNullOrWhiteSpace(w))
                     .GroupBy(w => w)
                     .ToDictionary(w => w.Key, w => w.Count())
                     .OrderByDescending(g => g.Value)
                     .Take(tagCloudWordAmount);
        }
    }
}