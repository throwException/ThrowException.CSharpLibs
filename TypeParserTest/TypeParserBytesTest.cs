using System;
using System.Collections.Generic;
using System.Globalization;
using ThrowException.CSharpLibs.BytesUtilLib;
using NUnit.Framework;
using ThrowException.CSharpLibs.TypeParserLib;

namespace ThrowException.CSharpLibs.TypeParserTest
{
    [TestFixture()]
    public class TypeParserBytesTest
    {
        [Test()]
        public void TryParse()
        {
            TypeParsers.ResetConfiguration();

            Assert.True(TypeParsers.TryParse("1337", out byte[] v1));
            Assert.AreEqual(v1, "1337".ParseHexBytes());

            Assert.True(TypeParsers.TryParse("cafeaffe", out byte[] v2));
            Assert.AreEqual(v2, "cafeaffe".ParseHexBytes());

            Assert.False(TypeParsers.TryParse("xx", out byte[] v3));
        }

        [Test()]
        public void ParseFail()
        {
            var parser = new TypeParserBytes();
            Assert.Throws<ValueParseException>(() => parser.Parse(".."));
            Assert.Throws<ValueParseException>(() => parser.Parse("§§"));
        }

        [Test()]
        public void Parse()
        {
            var parser = new TypeParserBytes();
            Assert.AreEqual(parser.Parse("1337"), "1337".ParseHexBytes());
            Assert.AreEqual(parser.Parse("cafeaffe"), "cafeaffe".ParseHexBytes());
            Assert.AreEqual(parser.Parse("0102030405060708090A0B0C0D"), "0102030405060708090A0B0C0D".ParseHexBytes());
        }

        [Test()]
        public void CanParse()
        {
            var parser = new TypeParserBytes();
            Assert.True(parser.CanParse("1337"));
            Assert.True(parser.CanParse("cafeaffe"));
            Assert.True(parser.CanParse("0102030405060708090A0B0C0D"));
            Assert.False(parser.CanParse("1"));
            Assert.False(parser.CanParse("caf"));
            Assert.False(parser.CanParse("XX"));
            Assert.False(parser.CanParse("zzzz"));
            Assert.False(parser.CanParse(".."));
            Assert.False(parser.CanParse("§§"));
            Assert.False(parser.CanParse("13.7"));
            Assert.False(parser.CanParse("caf§affe"));
            Assert.False(parser.CanParse("0102Z30405060708090A0B0C0D"));
        }

        [Test()]
        public void Format()
        {
            var parser = new TypeParserBytes();
            Assert.AreEqual("1337", parser.Format("1337".ParseHexBytes()));
            Assert.AreEqual("cafeaffe", parser.Format("cafeaffe".ParseHexBytes()));
            Assert.AreEqual("0102030405060708090a0b0c0d", parser.Format("0102030405060708090A0B0C0D".ParseHexBytes()));

            Assert.AreEqual("1337", TypeParsers.Format("1337".ParseHexBytes()));
        }
    }
}
