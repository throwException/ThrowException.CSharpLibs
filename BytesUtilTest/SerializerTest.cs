using ThrowException.CSharpLibs.BytesUtilLib;
using NUnit.Framework;
using System;
using System.IO;

namespace ThrowException.CSharpLibs.BytesUtilTest
{
    [TestFixture()]
    public class SerializerTest
    {
        [Test()]
        public void SerializerIntegers()
        {
            using (var serializer = new Serializer())
            {
                serializer.Write((byte)1);
                Assert.AreEqual(serializer.Data, new byte[] { 1 });
            }

            using (var serializer = new Serializer())
            {
                serializer.Write((sbyte)-1);
                Assert.AreEqual(serializer.Data, new byte[] { 0xff });
            }

            using (var serializer = new Serializer())
            {
                serializer.Write((short)1);
                Assert.AreEqual(serializer.Data, new byte[] { 0, 1 });
            }

            using (var serializer = new Serializer())
            {
                serializer.Write((short)-1);
                Assert.AreEqual(serializer.Data, new byte[] { 0xff, 0xff });
            }

            using (var serializer = new Serializer())
            {
                serializer.Write((ushort)1);
                Assert.AreEqual(serializer.Data, new byte[] { 0, 1 });
            }

            using (var serializer = new Serializer())
            {
                serializer.Write(1);
                Assert.AreEqual(serializer.Data, new byte[] { 0, 0, 0, 1 });
            }

            using (var serializer = new Serializer())
            {
                serializer.Write(-1);
                Assert.AreEqual(serializer.Data, new byte[] { 0xff, 0xff, 0xff, 0xff });
            }

            using (var serializer = new Serializer())
            {
                serializer.Write(1u);
                Assert.AreEqual(serializer.Data, new byte[] { 0, 0, 0, 1 });
            }

            using (var serializer = new Serializer())
            {
                serializer.Write(1L);
                Assert.AreEqual(serializer.Data, new byte[] { 0, 0, 0, 0, 0, 0, 0, 1 });
            }

            using (var serializer = new Serializer())
            {
                serializer.Write(-1L);
                Assert.AreEqual(serializer.Data, new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff });
            }

            using (var serializer = new Serializer())
            {
                serializer.Write(1UL);
                Assert.AreEqual(serializer.Data, new byte[] { 0, 0, 0, 0, 0, 0, 0, 1 });
            }
        }

        [Test()]
        public void SerializerFloats()
        {
            using (var serializer = new Serializer())
            {
                serializer.Write(1.25f);
                Assert.AreEqual(serializer.Data, BitConverter.GetBytes(1.25f));
            }

            using (var serializer = new Serializer())
            {
                serializer.Write(1.25d);
                Assert.AreEqual(serializer.Data, BitConverter.GetBytes(1.25d));
            }
        }

        [Test()]
        public void SerializerBool()
        {
            using (var serializer = new Serializer())
            {
                serializer.Write(true);
                Assert.AreEqual(serializer.Data, new byte[] { 0 });
            }

            using (var serializer = new Serializer())
            {
                serializer.Write(false);
                Assert.AreEqual(serializer.Data, new byte[] { 1 });
            }
        }

        [Test()]
        public void SerializerBytes()
        {
            using (var serializer = new Serializer())
            {
                serializer.Write(new byte[] { 1, 2 });
                Assert.AreEqual(serializer.Data, new byte[] { 1, 2 });
            }

            using (var serializer = new Serializer())
            {
                serializer.WritePrefixed(new byte[] { 1, 2 });
                Assert.AreEqual(serializer.Data, new byte[] { 0, 0, 0, 2, 1, 2 });
            }
        }

        [Test()]
        public void SerializerString()
        {
            using (var serializer = new Serializer())
            {
                serializer.WritePrefixed("hello");
                Assert.AreEqual(serializer.Data, new byte[] { 0, 0, 0, 5, (byte)'h', (byte)'e', (byte)'l', (byte)'l', (byte)'o' });
            }
        }

        [Test()]
        public void SerializerTimeSpan()
        {
            using (var serializer = new Serializer())
            {
                serializer.Write(TimeSpan.FromHours(1));
                Assert.AreEqual(serializer.Data, new byte[] { 0, 0, 0, 8, 97, 196, 104, 0 });
            }
        }

        [Test()]
        public void SerializerDateTime()
        {
            using (var serializer = new Serializer())
            {
                serializer.Write(new DateTime(2010, 1, 1));
                Assert.AreEqual(serializer.Data, new byte[] { 8, 204, 88, 140, 126, 229, 0, 0 });
            }
        }

        [Test()]
        public void SerializerGuid()
        {
            using (var serializer = new Serializer())
            {
                var guid = Guid.NewGuid();
                serializer.Write(guid);
                Assert.AreEqual(serializer.Data, guid.ToByteArray());
            }
        }

        [Test()]
        public void SerializerComplex()
        {
            using (var serializer = new Serializer())
            {
                serializer.Write(1);
                serializer.Write(-1);
                serializer.WritePrefixed(new byte[] { 0xca, 0xfe });
                Assert.AreEqual(serializer.Data,
                    new byte[] { 0, 0, 0, 1, 0xff, 0xff, 0xff, 0xff, 0, 0, 0, 2, 0xca, 0xfe });
            }
        }

        [Test()]
        public void SerializerStream()
        {
            using (var memory = new MemoryStream())
            {
                var serializer = new Serializer(memory);
                serializer.Write(1);
                serializer.Write(-1);
                serializer.WritePrefixed(new byte[] { 0xca, 0xfe });
                Assert.AreEqual(memory.ToArray(),
                    new byte[] { 0, 0, 0, 1, 0xff, 0xff, 0xff, 0xff, 0, 0, 0, 2, 0xca, 0xfe });
            }
        }
    }
}