using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ThrowException.CSharpLibs.BytesUtilLib;
using ThrowException.CSharpLibs.PipelineLib;

namespace ThrowException.CSharpLibs.PipelineTest
{
    [TestFixture()]
    public class BlockdeltaTests
    {
        private const string _block00file = "/tmp/block00";
        private const string _block01file = "/tmp/block01";
        private const string _block01ufile = "/tmp/block01u";

        [TearDown()]
        [SetUp()]
        public void Cleanup()
        {
            if (File.Exists(_block00file)) File.Delete(_block00file);
            if (File.Exists(_block01file)) File.Delete(_block01file);
            if (File.Exists(_block01ufile)) File.Delete(_block01ufile);
        }

        [Test()]
        public void S3UploadDownloadTest()
        {
            var random = new Random((int)DateTime.UtcNow.Ticks);
            var block00 = new byte[32 * 4096];
            random.NextBytes(block00);
            var block01 = block00.Copy();
            block01[10000] ^= 0xca;
            block01[30000] ^= 0xca;
            File.WriteAllBytes(_block00file, block00);
            File.WriteAllBytes(_block01file, block01);
            File.Copy(_block00file, _block01ufile);

            var createPipeline = new Pipeline();
            createPipeline.Add(new BlockdeltaCreateStage(new TestConfig(), _block00file, _block01file));
            createPipeline.Add(s => new ShortOutput(s));
            createPipeline.Start();
            createPipeline.WaitForDone();
            Assert.True(createPipeline.Done, "Blockdelta create done is not true");
            Assert.False(createPipeline.Failed, "Blockdelta create failed is not false");
            var update01 = createPipeline.Output.Data;

            var applyPipeline = new Pipeline();
            applyPipeline.Add(new InputStage(update01));
            applyPipeline.Add(new BlockdeltaApplyStage(new TestConfig(), _block01ufile));
            applyPipeline.Add(s => new ShortOutput(s));
            applyPipeline.Start();
            applyPipeline.WaitForDone();
            Assert.True(applyPipeline.Done, "Blockdelta apply done is not true");
            Assert.False(applyPipeline.Failed, "Blockdelta apply failed is not false");

            var block01u = File.ReadAllBytes(_block01ufile);
            Assert.AreEqual(block01, block01u, "Applid block mismatch");
        }
    }
}
