using System.Collections.Generic;

namespace TagCloud.Interfaces
{
    public interface ITagCloudTextPreprocessor
    {
        IEnumerable<KeyValuePair<string, int>> ProcessStrings(string[] text);
    }
}