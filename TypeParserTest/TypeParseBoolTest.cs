using System;
using System.Collections.Generic;
using System.Globalization;
using NUnit.Framework;
using ThrowException.CSharpLibs.TypeParserLib;

namespace ThrowException.CSharpLibs.TypeParserTest
{
    [TestFixture()]
    public class TypeParseBoolTest
    {
        [Test()]
        public void TryParse()
        {
            Assert.True(TypeParsers.TryParse("1", out bool v1));
            Assert.AreEqual(v1, true);
            Assert.True(TypeParsers.TryParse("T", out bool v2));
            Assert.AreEqual(v2, true);
            Assert.True(TypeParsers.TryParse("no", out bool v3));
            Assert.AreEqual(v3, false);
            Assert.False(TypeParsers.TryParse("xx", out bool v4));
        }

        [Test()]
        public void Parse()
        {
            var parser = new TypeParserBool();
            Assert.AreEqual(parser.Parse("0"), false);
            Assert.AreEqual(parser.Parse("1"), true);
            Assert.AreEqual(parser.Parse("f"), false);
            Assert.AreEqual(parser.Parse("t"), true);
            Assert.AreEqual(parser.Parse("false"), false);
            Assert.AreEqual(parser.Parse("true"), true);
            Assert.AreEqual(parser.Parse("n"), false);
            Assert.AreEqual(parser.Parse("y"), true);
            Assert.AreEqual(parser.Parse("no"), false);
            Assert.AreEqual(parser.Parse("yes"), true);
            Assert.AreEqual(parser.Parse("F"), false);
            Assert.AreEqual(parser.Parse("T"), true);
            Assert.AreEqual(parser.Parse("False"), false);
            Assert.AreEqual(parser.Parse("True"), true);
            Assert.AreEqual(parser.Parse("N"), false);
            Assert.AreEqual(parser.Parse("Y"), true);
            Assert.AreEqual(parser.Parse("No"), false);
            Assert.AreEqual(parser.Parse("Yes"), true);
        }

        [Test()]
        public void ParseFail()
        {
            var parser = new TypeParserBool();
            Assert.Throws<ValueParseException>(() => parser.Parse("xx"));
            Assert.Throws<ValueParseException>(() => parser.Parse("§§"));
        }

        [Test()]
        public void CanParse()
        {
            var parser = new TypeParserBool();
            Assert.True(parser.CanParse("0"));
            Assert.True(parser.CanParse("1"));
            Assert.True(parser.CanParse("f"));
            Assert.True(parser.CanParse("t"));
            Assert.True(parser.CanParse("false"));
            Assert.True(parser.CanParse("true"));
            Assert.True(parser.CanParse("n"));
            Assert.True(parser.CanParse("y"));
            Assert.True(parser.CanParse("no"));
            Assert.True(parser.CanParse("yes"));
            Assert.True(parser.CanParse("F"));
            Assert.True(parser.CanParse("T"));
            Assert.True(parser.CanParse("False"));
            Assert.True(parser.CanParse("True"));
            Assert.True(parser.CanParse("N"));
            Assert.True(parser.CanParse("Y"));
            Assert.True(parser.CanParse("No"));
            Assert.True(parser.CanParse("Yes"));
            Assert.False(parser.CanParse("x"));
            Assert.False(parser.CanParse("§"));
        }
    }
}
