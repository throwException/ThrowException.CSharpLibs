using ThrowException.CSharpLibs.BytesUtilLib;
using NUnit.Framework;
using System;
using System.IO;

namespace ThrowException.CSharpLibs.BytesUtilTest
{
    [TestFixture()]
    public class BytesTest
    {
        [Test()]
        public void ParseHexBytes()
        {
            Assert.AreEqual("1337".ParseHexBytes(), new byte[] { 0x13, 0x37 });
            Assert.AreEqual("cafebabe".ParseHexBytes(), new byte[] { 0xca, 0xfe, 0xba, 0xbe });

            Assert.Throws<FormatException>(() => "0q".ParseHexBytes());
            Assert.Throws<FormatException>(() => "0".ParseHexBytes());
            Assert.Throws<ArgumentNullException>(() => ((string)null).ParseHexBytes());
        }

        [Test()]
        public void ToHexString()
        {
            Assert.AreEqual(new byte[] { 0x13, 0x37 }.ToHexString(), "1337");
            Assert.AreEqual(new byte[] { 0xca, 0xfe, 0xba, 0xbe }.ToHexString(), "cafebabe");
            Assert.Throws<ArgumentNullException>(() => ((byte[])null).ToHexString());
        }


        [Test()]
        public void Concat()
        {
            Assert.AreEqual(new byte[] { 0x13, 0x37 }.Concat(new byte[] { 0x42, 0x23 }), new byte[] { 0x13, 0x37, 0x42, 0x23 });
            Assert.AreEqual(new byte[] { 0x13, 0x37 }.Concat(new byte[] { 0x42, 0x23 }, new byte[] { 0x42, 0x23 }), new byte[] { 0x13, 0x37, 0x42, 0x23, 0x42, 0x23 });
            Assert.Throws<ArgumentNullException>(() => ((byte[])null).Concat(new byte[0]));
            Assert.Throws<ArgumentNullException>(() => (new byte[0]).Concat((byte[])null));
        }

        [Test()]
        public void Part()
        {
            Assert.AreEqual(new byte[] { 0x13, 0x37, 0x42, 0x23 }.Part(0, 2), new byte[] { 0x13, 0x37 });
            Assert.AreEqual(new byte[] { 0x13, 0x37, 0x42, 0x23 }.Part(1, 2), new byte[] { 0x37, 0x42 });
            Assert.AreEqual(new byte[] { 0x13, 0x37, 0x42, 0x23 }.Part(0, 4), new byte[] { 0x13, 0x37, 0x42, 0x23 });
            Assert.AreEqual(new byte[] { 0x13, 0x37, 0x42, 0x23 }.Part(2, 2), new byte[] { 0x42, 0x23 });

            Assert.Throws<ArgumentNullException>(() => ((byte[])null).Part(3, 7));
            Assert.Throws<FormatException>(() => new byte[] { 0x13, 0x37, 0x42, 0x23 }.Part(-1, 1));
            Assert.Throws<FormatException>(() => new byte[] { 0x13, 0x37, 0x42, 0x23 }.Part(0, 5));
            Assert.Throws<FormatException>(() => new byte[] { 0x13, 0x37, 0x42, 0x23 }.Part(2, 3));
        }

        [Test()]
        public void AreEqual()
        {
            Assert.True(new byte[] { 0x13, 0x37, 0x42, 0x23 }.AreEqual(new byte[] { 0x13, 0x37, 0x42, 0x23 }));
            Assert.False(new byte[] { 0x13, 0x37, 0x42, 0x23 }.AreEqual(new byte[] { 0x13, 0x37, 0x42, 0x27 }));
            Assert.False(new byte[] { 0x13, 0x37, 0x42, 0x23 }.AreEqual(new byte[] { 0x13, 0x37, 0x42 }));
        }

        [Test()]
        public void Equal()
        {
            Assert.True(new byte[] { 0x13, 0x37, 0x42, 0x23 }.Equal(new byte[] { 0x13, 0x37, 0x42, 0x23 }));
            Assert.False(new byte[] { 0x13, 0x37, 0x42, 0x23 }.Equal(new byte[] { 0x13, 0x37, 0x42, 0x27 }));
            Assert.False(new byte[] { 0x13, 0x37, 0x42, 0x23 }.Equal(new byte[] { 0x13, 0x37, 0x42 }));
        }

        [Test()]
        public void ArgumentHasBytes()
        {
            new byte[] { 0x13, 0x37, 0x42, 0x23 }.ArgumentHasBytes(4);

            Assert.Throws<ArgumentNullException>(() => ((byte[])null).ArgumentHasBytes(3));
            Assert.Throws<ArgumentException>(() => new byte[] { 0x13, 0x37, 0x42, 0x23 }.ArgumentHasBytes(3));
            Assert.Throws<ArgumentException>(() => new byte[] { 0x13, 0x37, 0x42, 0x23 }.ArgumentHasBytes(5));
        }

        [Test()]
        public void Display()
        {
            new byte[] { 0x13, 0x37, 0x42, 0x23 }.Display("Test");

            Assert.Throws<ArgumentNullException>(() => ((byte[])null).Display(""));
            Assert.Throws<ArgumentNullException>(() => new byte[] { }.Display(null));
        }

        [Test()]
        public void Expand()
        {
            Assert.AreEqual(new byte[] { 0x13, 0x37, 0x42, 0x23 }
                .Expand(7), new byte[] { 0x13, 0x37, 0x42, 0x23, 0x00, 0x00, 0x00 });

            Assert.Throws<ArgumentNullException>(() => ((byte[])null).Expand(4));
        }

        [Test()]
        public void Pad()
        {
            Assert.AreEqual(new byte[] { 0x13, 0x37, 0x42, 0x23 }
                .Pad(4), new byte[] { 0x13, 0x37, 0x42, 0x23 });
            Assert.AreEqual(new byte[] { 0x13, 0x37, 0x42 }
                .Pad(4), new byte[] { 0x13, 0x37, 0x42, 0x00 });
            Assert.AreEqual(new byte[] { 0x13, 0x37 }
                .Pad(4), new byte[] { 0x13, 0x37, 0x00, 0x00 });

            Assert.Throws<ArgumentNullException>(() => ((byte[])null).Pad(4));
        }

        [Test()]
        public void PadInt()
        {
            Assert.AreEqual(1.PadInt(4), "0001");
            Assert.AreEqual(11.PadInt(4), "0011");
            Assert.AreEqual(111.PadInt(4), "0111");
            Assert.AreEqual(1111.PadInt(4), "1111");
            Assert.AreEqual(11111.PadInt(4), "11111");
        }

        [Test()]
        public void StartWith()
        {
            Assert.True(new byte[] { 0xca, 0xfe, 0xba, 0xbe }
                .StartWith(new byte[] { 0xca, 0xfe, 0xba, 0xbe }));
            Assert.True(new byte[] { 0xca, 0xfe, 0xba, 0xbe }
                .StartWith(new byte[] { 0xca, 0xfe, 0xba }));
            Assert.True(new byte[] { 0xca, 0xfe, 0xba, 0xbe }
                .StartWith(new byte[] { 0xca, 0xfe }));
            Assert.True(new byte[] { 0xca, 0xfe, 0xba, 0xbe }
                .StartWith(new byte[] { 0xca }));
            Assert.True(new byte[] { 0xca, 0xfe, 0xba, 0xbe }
                .StartWith(new byte[] { }));

            Assert.False(new byte[] { 0xca, 0xfe, 0xba, 0xbe }
                .StartWith(new byte[] { 0xca, 0x01, 0xba, 0xbe }));
            Assert.False(new byte[] { 0xca, 0xfe, 0xba, 0xbe }
                .StartWith(new byte[] { 0xca, 0x01, 0xba }));
            Assert.False(new byte[] { 0xca, 0xfe, 0xba, 0xbe }
                .StartWith(new byte[] { 0xca, 0x01 }));
            Assert.False(new byte[] { 0xca, 0xfe, 0xba, 0xbe }
                .StartWith(new byte[] { 0x01 }));

            Assert.Throws<ArgumentNullException>(() => ((byte[])null).StartWith(new byte[0]));
        }

        [Test()]
        public void ToBytes()
        {
            Assert.AreEqual(new sbyte[] { 0x1, 0x2 }.ToBytes(), new byte[] { 0x1, 0x2 });
            Assert.Throws<ArgumentNullException>(() => ((sbyte[])null).ToBytes());
        }

        [Test()]
        public void ToSbytes()
        {
            Assert.AreEqual(new byte[] { 0x1, 0x2 }.ToSbytes(), new sbyte[] { 0x1, 0x2 });
            Assert.Throws<ArgumentNullException>(() => ((byte[])null).ToSbytes());
        }

        [Test()]
        public void ToBytesFromStream()
        {
            var stream = new MemoryStream(new byte[] { 0xca, 0xfe, 0xba, 0xbe });
            Assert.AreEqual(stream.ToBytes(), new byte[] { 0xca, 0xfe, 0xba, 0xbe });
            Assert.Throws<ArgumentNullException>(() => ((Stream)null).ToBytes());
        }

        [Test()]
        public void ToBytesOrNull()
        {
            var stream = new MemoryStream(new byte[] { 0xca, 0xfe, 0xba, 0xbe });
            Assert.AreEqual(stream.ToBytesOrNull(), new byte[] { 0xca, 0xfe, 0xba, 0xbe });
            Assert.AreEqual(((Stream)null).ToBytesOrNull(), null);
        }

        [Test()]
        public void ToHexStringGroupFour()
        {
            Assert.AreEqual(new byte[] { 0xca, 0xfe, 0xba, 0xbe, 0x13, 0x37, 0x13 }.ToHexStringGroupFour(),
                "cafebabe 133713");
            Assert.AreEqual(new byte[] { 0xca, 0xfe, 0xba, 0xbe, 0x13, 0x37, 0x13, 0x37, 0x42 }.ToHexStringGroupFour(),
                "cafebabe 13371337 42");
            Assert.Throws<ArgumentNullException>(() => ((byte[])null).ToHexStringGroupFour());
        }

        [Test()]
        public void TryParseHexBytes()
        {
            Assert.AreEqual("cafebabe".TryParseHexBytes(), new byte[] { 0xca, 0xfe, 0xba, 0xbe });
            Assert.AreEqual("bad".TryParseHexBytes(), null);
            Assert.AreEqual("xx".TryParseHexBytes(), null);
            Assert.Throws<ArgumentNullException>(() => ((string)null).TryParseHexBytes());
        }

        [Test()]
        public void Xor()
        {
            Assert.AreEqual(new byte[] { 0x0, 0x1, 0x2 }
                .Xor(new byte[] { 0x88, 0x88, 0x88 }),
                new byte[] { 0x88, 0x89, 0x8a });
            Assert.Throws<ArgumentNullException>(() => ((byte[])null).Xor(new byte[0]));
            Assert.Throws<ArgumentNullException>(() => (new byte[0]).Xor(null));
        }

        [Test()]
        public void XorAdd()
        {
            var buffer = new byte[] { 0x0, 0x1, 0x2 };
            buffer.XorAdd(new byte[] { 0x88, 0x88, 0x88 });
            Assert.AreEqual(buffer, new byte[] { 0x88, 0x89, 0x8a });
            Assert.Throws<ArgumentNullException>(() => ((byte[])null).XorAdd(new byte[0]));
            Assert.Throws<ArgumentNullException>(() => (new byte[0]).XorAdd(null));
        }

        [Test()]
        public void Zeroize()
        {
            var buffer = new byte[] { 0x13, 0x37, 0x42, 0x23 };
            buffer.Zeroize();
            Assert.AreEqual(buffer, new byte[] { 0, 0, 0, 0 });

            Assert.Throws<ArgumentNullException>(() => ((byte[])null).Zeroize());
        }

        [Test()]
        public void ZeroizeInt()
        {
            var buffer = new int[] { 0x13, 0x37, 0x42, 0x23 };
            buffer.Zeroize();
            Assert.AreEqual(buffer, new int[] { 0, 0, 0, 0 });

            Assert.Throws<ArgumentNullException>(() => ((byte[])null).Zeroize());
        }

        [Test()]
        public void ZeroizeUint()
        {
            var buffer = new uint[] { 0x13, 0x37, 0x42, 0x23 };
            buffer.Zeroize();
            Assert.AreEqual(buffer, new uint[] { 0, 0, 0, 0 });

            Assert.Throws<ArgumentNullException>(() => ((byte[])null).Zeroize());
        }

        [Test()]
        public void FormatBytes()
        {
            Assert.AreEqual(0L.FormatBytes(), "0 Bytes");
            Assert.AreEqual(23L.FormatBytes(), "23 Bytes");
            Assert.AreEqual(1023L.FormatBytes(), "1023 Bytes");
            Assert.AreEqual(1024L.FormatBytes(), "1.0 KiB");
            Assert.AreEqual((1024L * 3).FormatBytes(), "3.0 KiB");
            Assert.AreEqual((1024L * 3 + 512).FormatBytes(), "3.5 KiB");
            Assert.AreEqual((1024L * 1024L).FormatBytes(), "1.00 MiB");
            Assert.AreEqual((1024L * 1024L * 3).FormatBytes(), "3.00 MiB");
            Assert.AreEqual((1024L * 1024L * 3 + 1024 * 512).FormatBytes(), "3.50 MiB");
            Assert.AreEqual((1024L * 1024L * 1024L).FormatBytes(), "1.00 GiB");
            Assert.AreEqual((1024L * 1024L * 1024L * 1024L).FormatBytes(), "1.00 TiB");
        }

        [Test()]
        public void FormatBytesOf()
        {
            Assert.AreEqual(0L.FormatBytesOf(0L), "0 / 0 Bytes");
            Assert.AreEqual(0L.FormatBytesOf(23L), "0 / 23 Bytes");
            Assert.AreEqual(0L.FormatBytesOf(1023L), "0 / 1023 Bytes");
            Assert.AreEqual(0L.FormatBytesOf(1024L), "0.0 / 1.0 KiB");
            Assert.AreEqual(1024L.FormatBytesOf(1024L), "1.0 / 1.0 KiB");
            Assert.AreEqual(0L.FormatBytesOf(1024L * 5L), "0.0 / 5.0 KiB");
            Assert.AreEqual((1024L * 2L).FormatBytesOf(1024L * 5L), "2.0 / 5.0 KiB");
            Assert.AreEqual(1L.FormatBytesOf(1024L * 1024L * 5L), "0.00 / 5.00 MiB");
            Assert.AreEqual(3L.FormatBytesOf(1024L * 1024L * 5L), "0.00 / 5.00 MiB");
            Assert.AreEqual((1024L * 1024L).FormatBytesOf(1024L * 1024L * 5L), "1.00 / 5.00 MiB");
            Assert.AreEqual(1024L.FormatBytesOf(1024L * 1024L * 1024L * 5L), "0.00 / 5.00 GiB");
            Assert.AreEqual(1337L.FormatBytesOf(1024L * 1024L * 1024L * 5L), "0.00 / 5.00 GiB");
            Assert.AreEqual((1024L * 1024L * 1024L * 4L).FormatBytesOf(1024L * 1024L * 1024L * 5L), "4.00 / 5.00 GiB");
            Assert.AreEqual(1337L.FormatBytesOf(1024L * 1024L * 1024L * 1024L * 5L), "0.00 / 5.00 TiB");
        }
    }
}
