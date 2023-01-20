using System;
using System.Collections.Generic;
using System.Globalization;
using NUnit.Framework;
using ThrowException.CSharpLibs.TypeParserLib;

namespace ThrowException.CSharpLibs.TypeParserTest
{
    [TestFixture()]
    public class TypeParserTimeSpanTest
    {
        [Test()]
        public void TryParse()
        {
            Assert.True(TypeParsers.TryParse("27h", out TimeSpan v1));
            Assert.AreEqual(v1, TimeSpan.FromHours(27d));

            Assert.True(TypeParsers.TryParse("23:59:59.111", out TimeSpan v2));
            Assert.AreEqual(v2, new TimeSpan(0, 23, 59, 59, 111));

            Assert.False(TypeParsers.TryParse("hello", out TimeSpan v3));
        }

        [Test()]
        public void ParseFail()
        {
            var parser = new TypeParserTimeSpan();
            Assert.Throws<ValueParseException>(() => parser.Parse("xx"));
            Assert.Throws<ValueParseException>(() => parser.Parse("§§"));
        }

        [Test()]
        public void Parse()
        {
            var parser = new TypeParserTimeSpan();
            Assert.AreEqual(parser.Parse("23:59:59"), new TimeSpan(0, 23, 59, 59));
            Assert.AreEqual(parser.Parse("23:59:59.1"), new TimeSpan(0, 23, 59, 59, 100));
            Assert.AreEqual(parser.Parse("23:59:59.11"), new TimeSpan(0, 23, 59, 59, 110));
            Assert.AreEqual(parser.Parse("23:59:59.111"), new TimeSpan(0, 23, 59, 59, 111));

            Assert.AreEqual(parser.Parse("113 23:59:59"), new TimeSpan(113, 23, 59, 59));
            Assert.AreEqual(parser.Parse("113 23:59:59.1"), new TimeSpan(113, 23, 59, 59, 100));
            Assert.AreEqual(parser.Parse("113 23:59:59.11"), new TimeSpan(113, 23, 59, 59, 110));
            Assert.AreEqual(parser.Parse("113 23:59:59.111"), new TimeSpan(113, 23, 59, 59, 111));
            Assert.AreEqual(parser.Parse("113.23:59:59"), new TimeSpan(113, 23, 59, 59));
            Assert.AreEqual(parser.Parse("113.23:59:59.1"), new TimeSpan(113, 23, 59, 59, 100));
            Assert.AreEqual(parser.Parse("113.23:59:59.11"), new TimeSpan(113, 23, 59, 59, 110));
            Assert.AreEqual(parser.Parse("113.23:59:59.111"), new TimeSpan(113, 23, 59, 59, 111));

            Assert.AreEqual(parser.Parse("27d"), TimeSpan.FromDays(27d));
            Assert.AreEqual(parser.Parse("27h"), TimeSpan.FromHours(27d));
            Assert.AreEqual(parser.Parse("27m"), TimeSpan.FromMinutes(27d));
            Assert.AreEqual(parser.Parse("27s"), TimeSpan.FromSeconds(27d));
            Assert.AreEqual(parser.Parse("27ms"), TimeSpan.FromMilliseconds(27d));
            Assert.AreEqual(parser.Parse("27qs"), TimeSpan.FromMilliseconds(0.027d));
            Assert.AreEqual(parser.Parse("27ns"), TimeSpan.FromMilliseconds(0.000027d));
        }

        [Test()]
        public void CanParse()
        {
            var parser = new TypeParserTimeSpan();
            Assert.True(parser.CanParse("23:59:59"));
            Assert.True(parser.CanParse("23:59:59.1"));
            Assert.True(parser.CanParse("23:59:59.11"));
            Assert.True(parser.CanParse("23:59:59.111"));

            Assert.False(parser.CanParse("25:59:59"));
            Assert.False(parser.CanParse("23:69:59.1"));
            Assert.False(parser.CanParse("23:59:79.11"));
            Assert.False(parser.CanParse("23:59:59.11x"));
            Assert.False(parser.CanParse("23:59:59.11y"));
            Assert.False(parser.CanParse("23:59:59.§"));

            Assert.True(parser.CanParse("113 23:59:59"));
            Assert.True(parser.CanParse("113 23:59:59.1"));
            Assert.True(parser.CanParse("113 23:59:59.11"));
            Assert.True(parser.CanParse("113 23:59:59.111"));

            Assert.False(parser.CanParse("113 26:59:59"));
            Assert.False(parser.CanParse("113 23:100:59.1"));
            Assert.False(parser.CanParse("113 23:59:61.11"));
            Assert.False(parser.CanParse("113 23:59:59.xx"));
            Assert.False(parser.CanParse("aa 23:59:59.111"));

            Assert.True(parser.CanParse("113.23:59:59"));
            Assert.True(parser.CanParse("113.23:59:59.1"));
            Assert.True(parser.CanParse("113.23:59:59.11"));
            Assert.True(parser.CanParse("113.23:59:59.111"));

            Assert.False(parser.CanParse("113.26:59:59"));
            Assert.False(parser.CanParse("113.23:100:59.1"));
            Assert.False(parser.CanParse("113.23:59:61.11"));
            Assert.False(parser.CanParse("113.23:59:59.xx"));
            Assert.False(parser.CanParse("aa.23:59:59.111"));

            Assert.True(parser.CanParse("27d"));
            Assert.True(parser.CanParse("27h"));
            Assert.True(parser.CanParse("27m"));
            Assert.True(parser.CanParse("27s"));
            Assert.True(parser.CanParse("27ms"));
            Assert.True(parser.CanParse("27qs"));
            Assert.True(parser.CanParse("27ns"));

            Assert.False(parser.CanParse("27x"));
            Assert.False(parser.CanParse("27q"));
            Assert.False(parser.CanParse("qqq"));
            Assert.False(parser.CanParse("..."));
            Assert.False(parser.CanParse("--27qs"));
        }
    }
}
