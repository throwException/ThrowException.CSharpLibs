using ThrowException.CSharpLibs.BytesUtilLib;
using NUnit.Framework;
using System;
using System.IO;

namespace ThrowException.CSharpLibs.BytesUtilTest
{
    [TestFixture()]
    public class DeserializerTest
    {
        [Test()]
        public void DeserializerIntegers()
        {
            using (var deserializer = new Deserializer(new byte[] { 1 }))
            {
                Assert.AreEqual(deserializer.ReadByte(), 1);
            }

            using (var deserializer = new Deserializer(new byte[] { 0xff }))
            {
                Assert.AreEqual(deserializer.ReadSByte(), -1);
            }

            using (var deserializer = new Deserializer(new byte[] { 0, 1 }))
            {
                Assert.AreEqual(deserializer.ReadInt16(), 1);
            }

            using (var deserializer = new Deserializer(new byte[] { 0xff, 0xff }))
            {
                Assert.AreEqual(deserializer.ReadInt16(), -1);
            }

            using (var deserializer = new Deserializer(new byte[] { 0, 1 }))
            {
                Assert.AreEqual(deserializer.ReadUint16(), 1);
            }

            using (var deserializer = new Deserializer(new byte[] { 0, 0, 0, 1 }))
            {
                Assert.AreEqual(deserializer.ReadInt32(), 1);
            }

            using (var deserializer = new Deserializer(new byte[] { 0xff, 0xff, 0xff, 0xff }))
            {
                Assert.AreEqual(deserializer.ReadInt32(), -1);
            }

            using (var deserializer = new Deserializer(new byte[] { 0, 0, 0, 1 }))
            {
                Assert.AreEqual(deserializer.ReadUInt32(), 1);
            }

            using (var deserializer = new Deserializer(new byte[] { 0, 0, 0, 0, 0, 0, 0, 1 }))
            {
                Assert.AreEqual(deserializer.ReadInt64(), 1);
            }

            using (var deserializer = new Deserializer(new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff }))
            {
                Assert.AreEqual(deserializer.ReadInt64(), -1);
            }

            using (var deserializer = new Deserializer(new byte[] { 0, 0, 0, 0, 0, 0, 0, 1 }))
            {
                Assert.AreEqual(deserializer.ReadUInt64(), 1);
            }
        }

        [Test()]
        public void DeserializerFloats()
        {
            using (var deserializer = new Deserializer(BitConverter.GetBytes(1.25d)))
            {
                Assert.AreEqual(deserializer.ReadDouble(), 1.25d);
            }
        }

        [Test()]
        public void DeserializerBool()
        {
            using (var deserializer = new Deserializer(new byte[] { 0 }))
            {
                Assert.True(deserializer.ReadBool());
            }

            using (var deserializer = new Deserializer(new byte[] { 1 }))
            {
                Assert.False(deserializer.ReadBool());
            }
        }

        [Test()]
        public void DeserializerBytes()
        {
            using (var deserializer = new Deserializer(new byte[] { 1, 2 }))
            {
                Assert.AreEqual(deserializer.ReadBytes(2), new byte[] { 1, 2 });
            }

            using (var deserializer = new Deserializer(new byte[] { 0, 0, 0, 2, 1, 2 }))
            {
                Assert.AreEqual(deserializer.ReadBytesPrefixed(), new byte[] { 1, 2 });
            }
        }

        [Test()]
        public void DeserializerString()
        {
            using (var deserializer = new Deserializer(new byte[] { 0, 0, 0, 5, (byte)'h', (byte)'e', (byte)'l', (byte)'l', (byte)'o' }))
            {
                Assert.AreEqual(deserializer.ReadStringPrefixed(), "hello");
            }
        }

        [Test()]
        public void DeserializerTimeSpan()
        {
            using (var deserializer = new Deserializer(new byte[] { 0, 0, 0, 8, 97, 196, 104, 0 }))
            {
                Assert.AreEqual(deserializer.ReadTimeSpan(), TimeSpan.FromHours(1));
            }
        }

        [Test()]
        public void DeserializerDateTime()
        {
            using (var deserializer = new Deserializer(new byte[] { 8, 204, 88, 140, 126, 229, 0, 0 }))
            {
                Assert.AreEqual(deserializer.ReadDateTime(), new DateTime(2010, 1, 1));
            }
        }

        [Test()]
        public void DeserializerGuid()
        {
            var guid = Guid.NewGuid();
            using (var deserializer = new Deserializer(guid.ToByteArray()))
            {
                Assert.AreEqual(deserializer.ReadGuid(), guid);
            }
        }

        [Test()]
        public void DeserializerComplex()
        {
            using (var deserializer = new Deserializer(new byte[] { 0, 0, 0, 1, 0xff, 0xff, 0xff, 0xff, 0, 0, 0, 2, 0xca, 0xfe }))
            {
                Assert.AreEqual(deserializer.ReadInt32(), 1);
                Assert.AreEqual(deserializer.ReadInt32(), -1);
                Assert.AreEqual(deserializer.ReadBytesPrefixed(), new byte[] { 0xca, 0xfe });
            }
        }

        [Test()]
        public void DeserializerStream()
        {
            using (var memory = new MemoryStream(new byte[] { 0, 0, 0, 1, 0xff, 0xff, 0xff, 0xff, 0, 0, 0, 2, 0xca, 0xfe }))
            {
                var deserializer = new Deserializer(memory);
                Assert.AreEqual(deserializer.ReadInt32(), 1);
                Assert.AreEqual(deserializer.ReadInt32(), -1);
                Assert.AreEqual(deserializer.ReadBytesPrefixed(), new byte[] { 0xca, 0xfe });
            }
        }
    }
}