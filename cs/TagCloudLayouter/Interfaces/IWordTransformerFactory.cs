using System.Collections.Generic;

namespace TagCloud.Interfaces
{
    public interface IWordTransformerFactory
    {
        SimpleWordTransformer CreateSimpleWordTransformer(IEnumerable<string> boringWords);
    }
}