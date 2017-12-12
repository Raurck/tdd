using System.IO;
using TagCloudLayouter.Interfaces;

namespace TagCloudLayouter
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