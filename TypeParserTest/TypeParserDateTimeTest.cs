using System;
using System.Collections.Generic;
using System.Globalization;
using NUnit.Framework;
using ThrowException.CSharpLibs.TypeParserLib;

namespace ThrowException.CSharpLibs.TypeParserTest
{
    [TestFixture()]
    public class TypeParserDateTimeTest
    {
        [Test()]
        public void TryParse()
        {
            Assert.True(TypeParsers.TryParse("2010-12-31", out DateTime v1));
            Assert.AreEqual(v1, DateTime.Parse("2010-12-31", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal));

            Assert.True(TypeParsers.TryParse("2010-12-31 23:59:59", out DateTime v2));
            Assert.AreEqual(v2, DateTime.Parse("2010-12-31T23:59:59", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal));

            Assert.False(TypeParsers.TryParse("hello", out DateTime v3));
        }

        [Test()]
        public void ParseFail()
        {
            var parser = new TypeParserDateTime();
            Assert.Throws<ValueParseException>(() => parser.Parse("xx"));
            Assert.Throws<ValueParseException>(() => parser.Parse("§§"));
        }

        [Test()]
        public void Parse()
        {
            var parser = new TypeParserDateTime();
            Assert.AreEqual(parser.Parse("2010-12-31"), DateTime.Parse("2010-12-31", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal));
            Assert.AreEqual(parser.Parse("2010-12-31 23:59:59"), DateTime.Parse("2010-12-31T23:59:59", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal));
            Assert.AreEqual(parser.Parse("2010-12-31T23:59:59"), DateTime.Parse("2010-12-31T23:59:59", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal));
        }

        [Test()]
        public void CanParse()
        {
            var parser = new TypeParserDateTime();
            Assert.True(parser.CanParse("2010-12-31"));
            Assert.True(parser.CanParse("2010-12-31 23:59:59"));
            Assert.True(parser.CanParse("2010-12-31T23:59:59"));

            Assert.False(parser.CanParse("2010-12-33"));
            Assert.False(parser.CanParse("2010-15-30"));
            Assert.False(parser.CanParse("2010-12"));
            Assert.False(parser.CanParse("2010-12-xx"));
            Assert.False(parser.CanParse("2010-12§§§"));

            Assert.False(parser.CanParse("2010-12-33 23:59:59"));
            Assert.False(parser.CanParse("2010-15-31 23:59:59"));
            Assert.False(parser.CanParse("2010-12-31 24:59:59"));
            Assert.False(parser.CanParse("2010-12-31 23:69:59"));
            Assert.False(parser.CanParse("2010-12-31 23:59:69"));
            Assert.False(parser.CanParse("2010-12-31 23:59:5Q"));
            Assert.False(parser.CanParse("2010-12-31 23:xx:59"));
            Assert.False(parser.CanParse("2010-12-31 2§:59:59"));
            Assert.False(parser.CanParse("2010-12-31X23:59:59"));
            Assert.False(parser.CanParse("2010-12-33T23:59:59"));
            Assert.False(parser.CanParse("2010-14-33T23:59:59"));
            Assert.False(parser.CanParse("2010-12-33T25:59:59"));
            Assert.False(parser.CanParse("2010-12-33T23:79:59"));
            Assert.False(parser.CanParse("2010-12-33T23:59:89"));
        }
    }
}
