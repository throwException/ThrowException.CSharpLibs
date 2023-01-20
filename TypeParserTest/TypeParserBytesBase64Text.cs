using System;
using System.Collections.Generic;
using System.Globalization;
using NUnit.Framework;
using ThrowException.CSharpLibs.TypeParserLib;

namespace ThrowException.CSharpLibs.TypeParserTest
{
    [TestFixture()]
    public class TypeParserBytesBase64Text
    {
        [Test()]
        public void TryParse()
        {
            TypeParsers.ResetConfiguration();
            var parser = TypeParsers.Get(typeof(TypeParserBytesBase64));
            parser.Enabled = true;
            parser.Precedence = 101;

            Assert.True(TypeParsers.TryParse("aGVsbG8=", out byte[] v1));
            Assert.AreEqual(v1, Convert.FromBase64String("aGVsbG8="));

            Assert.True(TypeParsers.TryParse("aGVsbG8gd29ybGQh", out byte[] v2));
            Assert.AreEqual(v2, Convert.FromBase64String("aGVsbG8gd29ybGQh"));

            Assert.False(TypeParsers.TryParse("...", out byte[] v3));
        }

        [Test()]
        public void ParseFail()
        {
            var parser = new TypeParserBytesBase64();
            Assert.Throws<ValueParseException>(() => parser.Parse(".."));
            Assert.Throws<ValueParseException>(() => parser.Parse("§§"));
        }

        [Test()]
        public void Parse()
        {
            var parser = new TypeParserBytesBase64();
            Assert.AreEqual(parser.Parse("aGVsbG8="), Convert.FromBase64String("aGVsbG8="));
            Assert.AreEqual(parser.Parse("aGVsbG8gd29ybGQh"), Convert.FromBase64String("aGVsbG8gd29ybGQh"));
            Assert.AreEqual(parser.Parse("YW5vdGhlciB0ZXh0IGluIGJ5dGVzLi4u"), Convert.FromBase64String("YW5vdGhlciB0ZXh0IGluIGJ5dGVzLi4u"));
        }

        [Test()]
        public void CanParse()
        {
            var parser = new TypeParserBytesBase64();
            Assert.True(parser.CanParse("aGVsbG8="));
            Assert.True(parser.CanParse("aGVsbG8gd29ybGQh"));
            Assert.True(parser.CanParse("YW5vdGhlciB0ZXh0IGluIGJ5dGVzLi4u"));
            Assert.False(parser.CanParse("..."));
            Assert.False(parser.CanParse("§§§§§"));
            Assert.False(parser.CanParse("aGVsb§8="));
            Assert.False(parser.CanParse("aGVsbG.gd29ybGQh"));
            Assert.False(parser.CanParse("YW5vdGh_ciB0ZXh0IGluIGJ5dGVzLi4u"));
        }
    }
}
