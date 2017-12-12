using System.Collections.Generic;
using System.Linq;
using TagCloudLayouter.Interfaces;

namespace TagCloudLayouter
{
    public class SimpleWordPreprocessor : IWordPreprocessor
    {
        private readonly IWordTransformer wordTransformer;
        private IEnumerable<char> wordSplitters = new[] { ' ', ',', '.', ';', ':', '\'', '-', '\n' };
        public SimpleWordPreprocessor(IWordTransformer wordTransformer)
        {
            this.wordTransformer = wordTransformer;
        }

        public Dictionary<string, int> ProcessStrings(string[] text)
        {
            return text
                     .SelectMany(str => str.Split(wordSplitters.ToArray()))
                     .Select(w => wordTransformer.GetTransformedWord(w))
                     .Where(w => !string.IsNullOrWhiteSpace(w))
                     .GroupBy(w => w)
                     .ToDictionary(w => w.Key, w => w.Count());
        }
    }
}