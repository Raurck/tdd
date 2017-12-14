using System.IO;
using TagCloud.Interfaces;

namespace TagCloud
{
    public class SimpleTextFileReader:ITextFileReader
    {
        private void IsExists(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException();
            }
        }

        public virtual string[] Read(string filePath)
        {
            IsExists(filePath);
            return File.ReadAllLines(filePath);
        }
    }
}