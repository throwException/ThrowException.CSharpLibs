using ThrowException.CSharpLibs.BytesUtilLib;
using NUnit.Framework;
using System;
using System.IO;

namespace ThrowException.CSharpLibs.BytesUtilTest
{
    [TestFixture()]
    public class ArgumentsTest
    {
        [Test()]
        public void ArgumentNotNull()
        {
            "".ArgumentNotNull();
            Assert.Throws<ArgumentNullException>(() => ((object)null).ArgumentNotNull());
        }

        [Test()]
        public void ArgumentInRange()
        {
            0.ArgumentInRange(0, 3);
            1.ArgumentInRange(0, 3);
            3.ArgumentInRange(0, 3);

            Assert.Throws<ArgumentOutOfRangeException>(() => 0.ArgumentInRange(1, 2));
        }

        [Test()]
        public void DisposeIfNotNull()
        {
            new MemoryStream().DisposeIfNotNull();
            ((Stream)null).DisposeIfNotNull();
        }
    }
}