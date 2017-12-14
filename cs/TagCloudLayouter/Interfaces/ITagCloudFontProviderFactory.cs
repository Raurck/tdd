using System.Collections.Generic;

namespace TagCloud.Interfaces
{
    public interface ITagCloudFontProviderFactory
    {
        TagCloudFontProvider CreateTagCloudFontProvider(IEnumerable<KeyValuePair<string, int>> words, int minFontSize, int maxFontSize);
    }
}