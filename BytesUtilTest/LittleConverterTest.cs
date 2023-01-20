using ThrowException.CSharpLibs.BytesUtilLib;
using NUnit.Framework;
using System;
using System.IO;

namespace ThrowException.CSharpLibs.BytesUtilTest
{
    [TestFixture()]
    public class LittleConverterTest
    {
        [Test()]
        public void GetBytes()
        {
            Assert.AreEqual(LittleConverter.GetBytes((short)1), new byte[] { 0, 1 });
            Assert.AreEqual(LittleConverter.GetBytes((short)0x0f0e), new byte[] { 0x0f, 0x0e });
            Assert.AreEqual(LittleConverter.GetBytes((short)-1), new byte[] { 0xff, 0xff });

            Assert.AreEqual(LittleConverter.GetBytes(1), new byte[] { 0, 0, 0, 1 });
            Assert.AreEqual(LittleConverter.GetBytes(0xf0e0d0c0), new byte[] { 0xf0, 0xe0, 0xd0, 0xc0 });
            Assert.AreEqual(LittleConverter.GetBytes(-1), new byte[] { 0xff, 0xff, 0xff, 0xff });

            Assert.AreEqual(LittleConverter.GetBytes(1L), new byte[] { 0, 0, 0, 0, 0, 0, 0, 1 });
            Assert.AreEqual(LittleConverter.GetBytes(0xf0e0d0c0b0a09080L), new byte[] { 0xf0, 0xe0, 0xd0, 0xc0, 0xb0, 0xa0, 0x90, 0x80 });
            Assert.AreEqual(LittleConverter.GetBytes(-1L), new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff });

            Assert.AreEqual(LittleConverter.GetBytes((ushort)1), new byte[] { 0, 1 });
            Assert.AreEqual(LittleConverter.GetBytes((ushort)0x0f0e), new byte[] { 0x0f, 0x0e });

            Assert.AreEqual(LittleConverter.GetBytes(1u), new byte[] { 0, 0, 0, 1 });
            Assert.AreEqual(LittleConverter.GetBytes(0xf0e0d0c0u), new byte[] { 0xf0, 0xe0, 0xd0, 0xc0 });

            Assert.AreEqual(LittleConverter.GetBytes(1ul), new byte[] { 0, 0, 0, 0, 0, 0, 0, 1 });
            Assert.AreEqual(LittleConverter.GetBytes(0xf0e0d0c0b0a09080ul), new byte[] { 0xf0, 0xe0, 0xd0, 0xc0, 0xb0, 0xa0, 0x90, 0x80 });
        }

        [Test()]
        public void ToInt()
        {
            Assert.AreEqual(LittleConverter.ToInt16(new byte[] { 0, 1 }), 1);
            Assert.AreEqual(LittleConverter.ToInt16(new byte[] { 0xff, 0xff }), -1);
            Assert.Throws<ArgumentException>(() => LittleConverter.ToInt16(new byte[] { 0 }));
            Assert.Throws<ArgumentNullException>(() => LittleConverter.ToInt16(null));

            Assert.AreEqual(LittleConverter.ToInt32(new byte[] { 0, 0, 0, 1 }), 1);
            Assert.AreEqual(LittleConverter.ToInt32(new byte[] { 0xff, 0xff, 0xff, 0xff }), -1);
            Assert.Throws<ArgumentException>(() => LittleConverter.ToInt32(new byte[] { 0 }));
            Assert.Throws<ArgumentNullException>(() => LittleConverter.ToInt32(null));

            Assert.AreEqual(LittleConverter.ToInt64(new byte[] { 0, 0, 0, 0, 0, 0, 0, 1 }), 1);
            Assert.AreEqual(LittleConverter.ToInt64(new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff }), -1);
            Assert.Throws<ArgumentException>(() => LittleConverter.ToInt64(new byte[] { 0 }));
            Assert.Throws<ArgumentNullException>(() => LittleConverter.ToInt64(null));

            Assert.AreEqual(LittleConverter.ToUInt16(new byte[] { 0, 1 }), 1);
            Assert.Throws<ArgumentException>(() => LittleConverter.ToUInt16(new byte[] { 0 }));
            Assert.Throws<ArgumentNullException>(() => LittleConverter.ToUInt16(null));

            Assert.AreEqual(LittleConverter.ToUInt32(new byte[] { 0, 0, 0, 1 }), 1);
            Assert.Throws<ArgumentException>(() => LittleConverter.ToUInt32(new byte[] { 0 }));
            Assert.Throws<ArgumentNullException>(() => LittleConverter.ToUInt32(null));

            Assert.AreEqual(LittleConverter.ToUInt64(new byte[] { 0, 0, 0, 0, 0, 0, 0, 1 }), 1);
            Assert.Throws<ArgumentException>(() => LittleConverter.ToUInt64(new byte[] { 0 }));
            Assert.Throws<ArgumentNullException>(() => LittleConverter.ToUInt64(null));
        }
    }
}