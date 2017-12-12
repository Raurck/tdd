using System.Collections.Generic;

namespace TagCloudLayouter.Interfaces
{
    public interface IWordPreprocessor
    {
        Dictionary<string, int> ProcessStrings(string[] text);
    }
}