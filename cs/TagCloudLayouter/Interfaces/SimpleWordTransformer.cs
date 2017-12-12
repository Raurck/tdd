namespace TagCloudLayouter.Interfaces
{
    public class SimpleWordTransformer:IWordTransformer
    {
        public string GetTransformedWord(string word)
        {
            return word;
        }
    }
}