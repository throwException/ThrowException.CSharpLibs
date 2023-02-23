using System;
using System.Collections.Generic;
using System.Globalization;
using NUnit.Framework;
using ThrowException.CSharpLibs.TypeParserLib;

namespace ThrowException.CSharpLibs.TypeParserTest
{
    [TestFixture()]
    public class TypeParseFloatingTest
    {
        [Test()]
        public void TryParse()
        {
            {
                Assert.True(TypeParsers.TryParse("0.23", out float v1));
                Assert.AreEqual(v1, 0.23f);
                Assert.True(TypeParsers.TryParse("-0.23", out float v2));
                Assert.AreEqual(v2, -0.23f);
                Assert.True(TypeParsers.TryParse("1337.42", out float v3));
                Assert.AreEqual(v3, 1337.42f);
                Assert.False(TypeParsers.TryParse("xx", out float v4));
            }

            {
                Assert.True(TypeParsers.TryParse("0.23", out double v1));
                Assert.AreEqual(v1, 0.23d);
                Assert.True(TypeParsers.TryParse("-0.23", out double v2));
                Assert.AreEqual(v2, -0.23d);
                Assert.True(TypeParsers.TryParse("1337.42", out double v3));
                Assert.AreEqual(v3, 1337.42d);
                Assert.False(TypeParsers.TryParse("xx", out double v4));
            }

            {
                Assert.True(TypeParsers.TryParse("0.23", out decimal v1));
                Assert.AreEqual(v1, 0.23M);
                Assert.True(TypeParsers.TryParse("-0.23", out decimal v2));
                Assert.AreEqual(v2, -0.23M);
                Assert.True(TypeParsers.TryParse("1337.42", out decimal v3));
                Assert.AreEqual(v3, 1337.42M);
                Assert.False(TypeParsers.TryParse("xx", out decimal v4));
            }
        }

        [Test()]
        public void ParseFail()
        {
            {
                var parser = new TypeParserFloat();
                Assert.Throws<ValueParseException>(() => parser.Parse("xx"));
                Assert.Throws<ValueParseException>(() => parser.Parse("§§"));
            }

            {
                var parser = new TypeParserDouble();
                Assert.Throws<ValueParseException>(() => parser.Parse("xx"));
                Assert.Throws<ValueParseException>(() => parser.Parse("§§"));
            }

            {
                var parser = new TypeParserDecimal();
                Assert.Throws<ValueParseException>(() => parser.Parse("xx"));
                Assert.Throws<ValueParseException>(() => parser.Parse("§§"));
            }
        }

        [Test()]
        public void Parse()
        {
            {
                var parser = new TypeParserFloat();
                Assert.AreEqual(parser.Parse("0"), 0);
                Assert.AreEqual(parser.Parse("1"), 1);
                Assert.AreEqual(parser.Parse("-1"), -1);
                Assert.AreEqual(parser.Parse("100"), 100);
                Assert.AreEqual(parser.Parse("-100"), -100);
                Assert.AreEqual(parser.Parse("1.0"), 1.0);
                Assert.AreEqual(parser.Parse("-1.0"), -1.0);
                Assert.AreEqual(parser.Parse("1.25"), 1.25);
                Assert.AreEqual(parser.Parse("-1.25"), -1.25);
            }

            {
                var parser = new TypeParserDouble();
                Assert.AreEqual(parser.Parse("0"), 0);
                Assert.AreEqual(parser.Parse("1"), 1);
                Assert.AreEqual(parser.Parse("-1"), -1);
                Assert.AreEqual(parser.Parse("100"), 100);
                Assert.AreEqual(parser.Parse("-100"), -100);
                Assert.AreEqual(parser.Parse("1.0"), 1.0);
                Assert.AreEqual(parser.Parse("-1.0"), -1.0);
                Assert.AreEqual(parser.Parse("1.25"), 1.25);
                Assert.AreEqual(parser.Parse("-1.25"), -1.25);
            }

            {
                var parser = new TypeParserDecimal();
                Assert.AreEqual(parser.Parse("0"), 0);
                Assert.AreEqual(parser.Parse("1"), 1);
                Assert.AreEqual(parser.Parse("-1"), -1);
                Assert.AreEqual(parser.Parse("100"), 100);
                Assert.AreEqual(parser.Parse("-100"), -100);
                Assert.AreEqual(parser.Parse("1.0"), 1.0);
                Assert.AreEqual(parser.Parse("-1.0"), -1.0);
                Assert.AreEqual(parser.Parse("1.25"), 1.25);
                Assert.AreEqual(parser.Parse("-1.25"), -1.25);
            }
        }

        [Test()]
        public void CanParse()
        {
            {
                var parser = new TypeParserFloat();
                Assert.True(parser.CanParse("0"));
                Assert.True(parser.CanParse("1"));
                Assert.True(parser.CanParse("-1"));
                Assert.True(parser.CanParse("100"));
                Assert.True(parser.CanParse("-100"));
                Assert.True(parser.CanParse("100.0"));
                Assert.True(parser.CanParse("-100.0"));
                Assert.True(parser.CanParse("100.23"));
                Assert.True(parser.CanParse("-100.23"));
                Assert.False(parser.CanParse("x"));
                Assert.False(parser.CanParse("§"));
            }

            {
                var parser = new TypeParserDouble();
                Assert.True(parser.CanParse("0"));
                Assert.True(parser.CanParse("1"));
                Assert.True(parser.CanParse("-1"));
                Assert.True(parser.CanParse("100"));
                Assert.True(parser.CanParse("-100"));
                Assert.True(parser.CanParse("100.0"));
                Assert.True(parser.CanParse("-100.0"));
                Assert.True(parser.CanParse("100.23"));
                Assert.True(parser.CanParse("-100.23"));
                Assert.False(parser.CanParse("x"));
                Assert.False(parser.CanParse("§"));
            }

            {
                var parser = new TypeParserDecimal();
                Assert.True(parser.CanParse("0"));
                Assert.True(parser.CanParse("1"));
                Assert.True(parser.CanParse("-1"));
                Assert.True(parser.CanParse("100"));
                Assert.True(parser.CanParse("-100"));
                Assert.True(parser.CanParse("100.0"));
                Assert.True(parser.CanParse("-100.0"));
                Assert.True(parser.CanParse("100.23"));
                Assert.True(parser.CanParse("-100.23"));
                Assert.False(parser.CanParse("x"));
                Assert.False(parser.CanParse("§"));
            }
        }

        [Test()]
        public void Format()
        {
            {
                var parser = new TypeParserFloat();
                Assert.AreEqual("0", parser.Format(0));
                Assert.AreEqual("1", parser.Format(1));
                Assert.AreEqual("-1", parser.Format(-1));
                Assert.AreEqual("100", parser.Format(100));
                Assert.AreEqual("-100", parser.Format(-100));
                Assert.AreEqual("1", parser.Format(1.0f));
                Assert.AreEqual("-1", parser.Format(-1.0f));
                Assert.AreEqual("1.25", parser.Format(1.25f));
                Assert.AreEqual("-1.25", parser.Format(-1.25f));
            }

            {
                var parser = new TypeParserDouble();
                Assert.AreEqual("0", parser.Format(0));
                Assert.AreEqual("1", parser.Format(1));
                Assert.AreEqual("-1", parser.Format(-1));
                Assert.AreEqual("100", parser.Format(100));
                Assert.AreEqual("-100", parser.Format(-100));
                Assert.AreEqual("1", parser.Format(1.0d));
                Assert.AreEqual("-1", parser.Format(-1.0d));
                Assert.AreEqual("1.25", parser.Format(1.25d));
                Assert.AreEqual("-1.25", parser.Format(-1.25d));
            }

            {
                var parser = new TypeParserDecimal();
                Assert.AreEqual("0", parser.Format(0));
                Assert.AreEqual("1", parser.Format(1));
                Assert.AreEqual("-1", parser.Format(-1));
                Assert.AreEqual("100", parser.Format(100));
                Assert.AreEqual("-100", parser.Format(-100));
                Assert.AreEqual("1.0", parser.Format(1.0M));
                Assert.AreEqual("-1.0", parser.Format(-1.0M));
                Assert.AreEqual("1.25", parser.Format(1.25M));
                Assert.AreEqual("-1.25", parser.Format(-1.25M));
            }

            {
                Assert.AreEqual("-1.25", TypeParsers.Format(-1.25M));
                Assert.AreEqual("-1.25", TypeParsers.Format(-1.25d));
                Assert.AreEqual("-1.25", TypeParsers.Format(-1.25f));
            }
        }
    }
}
