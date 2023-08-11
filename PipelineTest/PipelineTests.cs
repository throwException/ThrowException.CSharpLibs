using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using ThrowException.CSharpLibs.BytesUtilLib;
using ThrowException.CSharpLibs.PipelineLib;

namespace ThrowException.CSharpLibs.PipelineTest
{
    [TestFixture()]
    public class PipelineTests
    {
        [Test()]
        public void TwoStageTest()
        {
            var pipeline = new Pipeline();
            pipeline.Add(new ProcessStage("dd-zero", "/usr/bin/dd", "if=/dev/zero bs=1M count=16"));
            pipeline.Add(new HashStage(new TestConfig(), OpensslHash.SHA256), true);
            pipeline.Add(s => new ShortOutput(s));
            pipeline.Start();
            pipeline.WaitForDone();
            Assert.True(pipeline.Done, "Done is not true");
            Assert.False(pipeline.Failed, "Failed is not false");
            Assert.AreEqual(pipeline.TotalBytes, 16 * 1024 * 1024, "Total bytes do not match");
            Assert.AreEqual(pipeline.Output.Hex, "080acf35a507ac9849cfcba47dc2ad83e01b75663a516279c8b9d243b719643e");
        }

        [Test()]
        public void TwoStageTestLong()
        {
            var pipeline = new Pipeline();
            pipeline.Add(new ProcessStage("dd-zero", "/usr/bin/dd", "if=/dev/zero bs=1M count=2048"));
            pipeline.Add(new ProcessStage("dd-null", "/usr/bin/dd", "of=/dev/null bs=1M"), true);
            pipeline.Add(s => new ShortOutput(s));
            pipeline.Start();
            pipeline.WaitForDone();
            Assert.True(pipeline.Done, "Done is not true");
            Assert.False(pipeline.Failed, "Failed is not false");
            Assert.AreEqual(pipeline.TotalBytes, 2048L * 1024 * 1024, "Total bytes do not match");
        }

        [Test()]
        public void TwoStageFailureOneTest()
        {
            var pipeline = new Pipeline();
            pipeline.Add(new ProcessStage("dd-zero", "/usr/bin/dd", "quarkquarkquark"));
            pipeline.Add(new HashStage(new TestConfig(), OpensslHash.SHA256));
            pipeline.Add(s => new ShortOutput(s));
            pipeline.Start();
            pipeline.WaitForDone();
            Assert.True(pipeline.Done, "Done is not true");
            Assert.True(pipeline.Failed, "Failed is not true");
        }

        [Test()]
        public void TwoStageFailureTwoTest()
        {
            var pipeline = new Pipeline();
            pipeline.Add(new ProcessStage("dd-zero", "/usr/bin/dd", "if=/dev/zero bs=1M count=16"));
            pipeline.Add(new ProcessStage("sha256sum", "/usr/bin/sha256sum", "quarkquarkquark"));
            pipeline.Add(s => new ShortOutput(s));
            pipeline.Start();
            pipeline.WaitForDone();
            Assert.True(pipeline.Done, "Done is not true");
            Assert.True(pipeline.Failed, "Failed is not true");
        }

        [Test()]
        public void ThreeStageTest()
        {
            var pipeline = new Pipeline();
            pipeline.Add(new ProcessStage("dd-zero", "/usr/bin/dd", "if=/dev/zero bs=1M count=16"));
            pipeline.Add(new ProcessStage("openssl", "/usr/bin/openssl", "aes-256-cbc -K 0000000000000000000000000000000000000000000000000000000000000000 -iv 00000000000000000000000000000000"), true);
            pipeline.Add(new HashStage(new TestConfig(), OpensslHash.SHA256));
            pipeline.Add(s => new ShortOutput(s));
            pipeline.Start();
            pipeline.WaitForDone();
            Assert.True(pipeline.Done, "Done is not true");
            Assert.False(pipeline.Failed, "Failed is not false");
            Assert.AreEqual(pipeline.TotalBytes, 16 * 1024 * 1024, "Total bytes do not match");
            Assert.AreEqual(pipeline.Output.Hex, "e738cf9545d7a6fa2071f979f55c7c27cfd12b1135689eea3c7823fd887336be");
        }

        [Test()]
        public void ThreeStageTestLong()
        {
            var pipeline = new Pipeline();
            pipeline.Add(new ProcessStage("dd-zero", "/usr/bin/dd", "if=/dev/zero bs=1M count=256"));
            pipeline.Add(new ProcessStage("openssl", "/usr/bin/openssl", "aes-256-cbc -K 0000000000000000000000000000000000000000000000000000000000000000 -iv 00000000000000000000000000000000"), true);
            pipeline.Add(new HashStage(new TestConfig(), OpensslHash.SHA256));
            pipeline.Add(s => new ShortOutput(s));
            pipeline.Start();
            pipeline.WaitForDone();
            Assert.True(pipeline.Done, "Done is not true");
            Assert.False(pipeline.Failed, "Failed is not false");
            Assert.AreEqual(pipeline.TotalBytes, 256 * 1024 * 1024, "Total bytes do not match");
            Assert.AreEqual(pipeline.Output.Hex, "c1180de530385bcc5f18e3759a5449ab4f92147b455e2dcc006c1c58c4a79bfa");
        }

        [Test()]
        public void ThreeStageTestFailureOneTest()
        {
            var pipeline = new Pipeline();
            pipeline.Add(new ProcessStage("dd-zero", "/usr/bin/dd", "if=/dev/zero bs=1M count=16 xxxxxxxxxxx"));
            pipeline.Add(new ProcessStage("openssl", "/usr/bin/openssl", "aes-256-cbc -K 0000000000000000000000000000000000000000000000000000000000000000 -iv 00000000000000000000000000000000"));
            pipeline.Add(new HashStage(new TestConfig(), OpensslHash.SHA256));
            pipeline.Add(s => new ShortOutput(s));
            pipeline.Start();
            pipeline.WaitForDone();
            Assert.True(pipeline.Done, "Done is not true");
            Assert.True(pipeline.Failed, "Failed is not true");
        }

        [Test()]
        public void ThreeStageTestFailureTwoTest()
        {
            var pipeline = new Pipeline();
            pipeline.Add(new ProcessStage("dd-zero", "/usr/bin/dd", "if=/dev/zero bs=1M count=16"));
            pipeline.Add(new ProcessStage("openssl", "/usr/bin/openssl", "aes-256-cbc qqqqqqqqqqqqq -K 0000000000000000000000000000000000000000000000000000000000000000 -iv 00000000000000000000000000000000"));
            pipeline.Add(new HashStage(new TestConfig(), OpensslHash.SHA256));
            pipeline.Add(s => new ShortOutput(s));
            pipeline.Start();
            pipeline.WaitForDone();
            Assert.True(pipeline.Done, "Done is not true");
            Assert.True(pipeline.Failed, "Failed is not true");
        }

        [Test()]
        public void ThreeStageTestFailureThreeTest()
        {
            var pipeline = new Pipeline();
            pipeline.Add(new ProcessStage("dd-zero", "/usr/bin/dd", "if=/dev/zero bs=1M count=16"));
            pipeline.Add(new ProcessStage("openssl", "/usr/bin/openssl", "aes-256-cbc -K 0000000000000000000000000000000000000000000000000000000000000000 -iv 00000000000000000000000000000000"));
            pipeline.Add(new ProcessStage("sha256sum", "/usr/bin/sha256sum", "oops"));
            pipeline.Add(s => new ShortOutput(s));
            pipeline.Start();
            pipeline.WaitForDone();
            Assert.True(pipeline.Done, "Done is not true");
            Assert.True(pipeline.Failed, "Failed is not true");
        }

        [Test()]
        public void SixStageTest()
        {
            var pipeline = new Pipeline();
            pipeline.Add(new ProcessStage("dd-zero", "/usr/bin/dd", "if=/dev/zero bs=1M count=16"));
            pipeline.Add(new GzipStage(new TestConfig(), false), true);
            pipeline.Add(new ProcessStage("cat", "/usr/bin/cat", string.Empty));
            pipeline.Add(new GzipStage(new TestConfig(), true));
            pipeline.Add(new ProcessStage("openssl", "/usr/bin/openssl", "aes-256-cbc -K 0000000000000000000000000000000000000000000000000000000000000000 -iv 00000000000000000000000000000000"));
            pipeline.Add(new HashStage(new TestConfig(), OpensslHash.SHA256));
            pipeline.Add(s => new ShortOutput(s));
            pipeline.Start();
            pipeline.WaitForDone();
            Assert.True(pipeline.Done, "Done is not true");
            Assert.False(pipeline.Failed, "Failed is not false");
            Assert.AreEqual(pipeline.TotalBytes, 16 * 1024 * 1024, "Total bytes do not match");
            Assert.AreEqual(pipeline.Output.Hex, "e738cf9545d7a6fa2071f979f55c7c27cfd12b1135689eea3c7823fd887336be");
        }

        [Test()]
        public void MultiStageTest()
        {
            var pipeline = new Pipeline();
            pipeline.Add(new ProcessStage("dd-zero", "/usr/bin/dd", "if=/dev/zero bs=1M count=16"));
            pipeline.Add(new GzipStage(new TestConfig(), false), true);
            pipeline.Add(new EncryptStage(new TestConfig(), OpensslCipher.Chacha20, "test", false));
            pipeline.Add(new StreamAuthStage(new TestConfig(), "cafebabe", false));
            pipeline.Add(new ProcessStage("cat", "/usr/bin/cat", string.Empty));
            pipeline.Add(new StreamAuthStage(new TestConfig(), "cafebabe", true));
            pipeline.Add(new EncryptStage(new TestConfig(), OpensslCipher.Chacha20, "test", true));
            pipeline.Add(new GzipStage(new TestConfig(), true));
            pipeline.Add(new HashStage(new TestConfig(), OpensslHash.SHA256));
            pipeline.Add(s => new ShortOutput(s));
            pipeline.Start();
            pipeline.WaitForDone();
            Assert.True(pipeline.Done, "Done is not true");
            Assert.False(pipeline.Failed, "Failed is not false");
            Assert.AreEqual(pipeline.TotalBytes, 16 * 1024 * 1024, "Total bytes do not match");
            Assert.AreEqual(pipeline.Output.Hex, "080acf35a507ac9849cfcba47dc2ad83e01b75663a516279c8b9d243b719643e");
        }

        [Test()]
        public void StreamAuthTest()
        {
            var pipeline = new Pipeline();
            pipeline.Add(new ProcessStage("dd-zero", "/usr/bin/dd", "if=/dev/zero bs=1M count=16"));
            pipeline.Add(new StreamAuthStage(new TestConfig(), "cafebabe", false), true);
            pipeline.Add(new StreamAuthStage(new TestConfig(), "cafebabe", true));
            pipeline.Add(new HashStage(new TestConfig(), OpensslHash.SHA256));
            pipeline.Add(s => new ShortOutput(s));
            pipeline.Start();
            pipeline.WaitForDone();
            Assert.True(pipeline.Done, "Done is not true");
            Assert.False(pipeline.Failed, "Failed is not false");
            Assert.AreEqual(pipeline.TotalBytes, 16 * 1024 * 1024, "Total bytes do not match");
            Assert.AreEqual(pipeline.Output.Hex, "080acf35a507ac9849cfcba47dc2ad83e01b75663a516279c8b9d243b719643e");
        }

        [Test()]
        public void StreamAuthTestFailureKey()
        {
            var pipeline = new Pipeline();
            pipeline.Add(new ProcessStage("dd-zero", "/usr/bin/dd", "if=/dev/zero bs=1M count=16"));
            pipeline.Add(new StreamAuthStage(new TestConfig(), "cafebabe", false), true);
            pipeline.Add(new StreamAuthStage(new TestConfig(), "cafeba", true));
            pipeline.Add(new HashStage(new TestConfig(), OpensslHash.SHA256));
            pipeline.Add(s => new ShortOutput(s));
            pipeline.Start();
            pipeline.WaitForDone();
            Assert.True(pipeline.Done, "Done is not true");
            Assert.True(pipeline.Failed, "Failed is not true");
        }

        [Test()]
        public void StreamAuthTestFailureData()
        {
            var pipeline = new Pipeline();
            pipeline.Add(new ProcessStage("dd-zero", "/usr/bin/dd", "if=/dev/zero bs=1M count=16"));
            pipeline.Add(new StreamAuthStage(new TestConfig(), "cafebabe", false), true);
            pipeline.Add(new EncryptStage(new TestConfig(), OpensslCipher.Camellia256CTR, "cafebabe", false), true);
            pipeline.Add(new StreamAuthStage(new TestConfig(), "cafebabe", true));
            pipeline.Add(new HashStage(new TestConfig(), OpensslHash.SHA256));
            pipeline.Add(s => new ShortOutput(s));
            pipeline.Start();
            pipeline.WaitForDone();
            Assert.True(pipeline.Done, "Done is not true");
            Assert.True(pipeline.Failed, "Failed is not true");
        }

        [Test()]
        public void FileTest()
        {
            var filename = "/tmp/" + Bytes.Random(16).ToHexString();

            using (var pipeline = new Pipeline())
            {
                pipeline.Add(new ProcessStage("dd-zero", "/usr/bin/dd", "if=/dev/zero bs=1M count=16"));
                pipeline.Add(new EncryptStage(new TestConfig(), OpensslCipher.Chacha20, "cafebabe", false), true);
                pipeline.Add(s => new FileOutput(s, filename));
                pipeline.Start();
                pipeline.WaitForDone();
                Assert.True(pipeline.Done, "Done is not true");
                Assert.False(pipeline.Failed, "Failed is not false");
                Assert.AreEqual(pipeline.TotalBytes, 16 * 1024 * 1024, "Total bytes do not match");
            }

            using (var pipeline = new Pipeline())
            {
                pipeline.Add(new DumpStage(filename));
                pipeline.Add(new EncryptStage(new TestConfig(), OpensslCipher.Chacha20, "cafebabe", true), true);
                pipeline.Add(s => new ShortOutput(s));
                pipeline.Start();
                pipeline.WaitForDone();
                Assert.True(pipeline.Done, "Done is not true");
                Assert.False(pipeline.Failed, "Failed is not false");
                Assert.AreEqual(pipeline.Output.Data.Length, 16 * 1024 * 1024, "Output data length does not match");
                Assert.True(pipeline.Output.Data.All(x => x == 0), "Output data is not zero");
            }

            File.Delete(filename);
        }
    }
}
