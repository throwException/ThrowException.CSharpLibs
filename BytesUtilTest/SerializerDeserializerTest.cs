using ThrowException.CSharpLibs.BytesUtilLib;
using NUnit.Framework;
using System;
using System.IO;

namespace ThrowException.CSharpLibs.BytesUtilTest
{
    [TestFixture()]
    public class SerializerDeserializerTest
    {
        [Test()]
        public void CaseOne()
        {
            using (var serializer = new Serializer())
            {
                serializer.Write(true);
                serializer.Write(false);
                serializer.Write(23);
                serializer.Write(TimeSpan.FromHours(2.7d));
                serializer.Write(long.MaxValue);
                serializer.WritePrefixed("hello");
                serializer.Write(new byte[] { 0xca, 0xfe });
                serializer.WritePrefixed(new byte[] { 0x13, 0x37 });
                using (var deserializer = new Deserializer(serializer.Data))
                {
                    Assert.True(deserializer.ReadBool());
                    Assert.False(deserializer.ReadBool());
                    Assert.AreEqual(deserializer.ReadInt32(), 23);
                    Assert.AreEqual(deserializer.ReadTimeSpan(), TimeSpan.FromHours(2.7d));
                    Assert.AreEqual(deserializer.ReadInt64(), long.MaxValue);
                    Assert.AreEqual(deserializer.ReadStringPrefixed(), "hello");
                    Assert.AreEqual(deserializer.ReadBytes(2), new byte[] { 0xca, 0xfe });
                    Assert.AreEqual(deserializer.ReadBytesPrefixed(), new byte[] { 0x13, 0x37 });
                }
            }
        }

        [Test()]
        public void CaseTwo()
        {
            using (var serializer = new Serializer())
            {
                serializer.WritePrefixed("hello");
                serializer.Write(134132412u);
                serializer.Write(ulong.MaxValue);
                serializer.Write(TimeSpan.FromMilliseconds(2.7d));
                serializer.WritePrefixed(new byte[] { 0x13, 0x37 });
                serializer.Write(new byte[] { 0xca, 0xfe });
                using (var deserializer = new Deserializer(serializer.Data))
                {
                    Assert.AreEqual(deserializer.ReadStringPrefixed(), "hello");
                    Assert.AreEqual(deserializer.ReadUInt32(), 134132412u);
                    Assert.AreEqual(deserializer.ReadUInt64(), ulong.MaxValue);
                    Assert.AreEqual(deserializer.ReadTimeSpan(), TimeSpan.FromMilliseconds(2.7d));
                    Assert.AreEqual(deserializer.ReadBytesPrefixed(), new byte[] { 0x13, 0x37 });
                    Assert.AreEqual(deserializer.ReadBytes(2), new byte[] { 0xca, 0xfe });
                }
            }
        }
    }
}