using System.Collections.Generic;

namespace TagCloudLayouter.Interfaces
{
    public interface ITagCloudTextPreprocessor
    {
        IEnumerable<KeyValuePair<string, int>> ProcessStrings(string[] text);
    }
}