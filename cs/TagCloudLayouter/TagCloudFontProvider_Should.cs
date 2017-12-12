using System;
using System.Collections.Generic;
using NUnit.Framework;
using FluentAssertions;
using NUnit.Framework.Internal;

namespace TagCloudLayouter
{
    [TestFixture]
    public class TagCloudFontProvider_Should
    {
        [Test]
        public void TagCloudFontProvider_ShouldThrowErrorOnMinMoreThenMax()
        {
            Action act = () => new TagCloudFontProvider(new Dictionary<string, int>{{"test",1}}, 10, 5);
            act.ShouldThrow<ArgumentOutOfRangeException>();
        }

        [Test]
        public void TagCloudFontProvider_ShouldThrowErrorOnNullDictonary()
        {
            Action act = () => new TagCloudFontProvider(null, 10, 5);
            act.ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void TagCloudFontProvider_ShouldThrowErrorOnEmptyDictonary()
        {
            Action act = () => new TagCloudFontProvider(new Dictionary<string, int>(), 10, 5);
            act.ShouldThrow<ArgumentException>();
        }

    }
}