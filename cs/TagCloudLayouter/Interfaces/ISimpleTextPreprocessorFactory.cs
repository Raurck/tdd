using System.Collections.Generic;

namespace TagCloud.Interfaces
{
    public interface ISimpleTextPreprocessorFactory
    {
        SimplePreprocessor CreateSimplePreprocessor(int tagCloudWordAmount, IEnumerable<char> wordSplitters);
    }
}