using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ThrowException.CSharpLibs.PipelineLib;

namespace ThrowException.CSharpLibs.PipelineTest
{
    [TestFixture()]
    public class S3Tests
    {
        private class S3Config
        { 
            public string EndpointUrl { get; private set; }
            public string AwsAccessKeyId { get; private set; }
            public string AwsSecretAccessKey { get; private set; }
            public string Bucket { get; private set; }
            public string Key { get; private set; }

            public S3Config(string filename)
            {
                var lines = new Queue<string>(File.ReadLines(filename));
                EndpointUrl = lines.Dequeue();
                AwsAccessKeyId = lines.Dequeue();
                AwsSecretAccessKey = lines.Dequeue();
                Bucket = lines.Dequeue();
                Key = lines.Dequeue();
            }
        }

        [Test()]
        public void S3UploadDownloadTest()
        {
            const string filename = "/home/user/dev/config/development/s3/wasabi/user.set.testing.savvy.ch.txt";
            if (!File.Exists(filename))
            {
                Assert.Inconclusive("Configuration file not available");
                return;
            }

            var config = new S3Config(filename);

            var random = new Random((int)DateTime.UtcNow.Ticks);
            var data = new byte[16];
            random.NextBytes(data);

            var uploadPipeline = new Pipeline();
            uploadPipeline.Add(new InputStage(data));
            uploadPipeline.Add(new S3UploadStage(config.EndpointUrl, config.AwsAccessKeyId, config.AwsSecretAccessKey, config.Bucket, config.Key, 32), true);
            uploadPipeline.Add(s => new ShortOutput(s));
            uploadPipeline.Start();
            uploadPipeline.WaitForDone();
            Assert.True(uploadPipeline.Done, "Upload done is not true");
            Assert.False(uploadPipeline.Failed, "Upload failed is not false");
            Assert.AreEqual(uploadPipeline.TotalBytes, 16, "Upload total bytes do not match");

            var downloadPipeline = new Pipeline();
            downloadPipeline.Add(new S3DownloadStage(config.EndpointUrl, config.AwsAccessKeyId, config.AwsSecretAccessKey, config.Bucket, config.Key));
            downloadPipeline.Add(new ProcessStage("cat", "/usr/bin/cat", string.Empty), true);
            downloadPipeline.Add(s => new ShortOutput(s));
            downloadPipeline.Start();
            downloadPipeline.WaitForDone();
            Assert.True(downloadPipeline.Done, "Download done is not true");
            Assert.False(downloadPipeline.Failed, "Download failed is not false");
            Assert.AreEqual(downloadPipeline.TotalBytes, 16, "Download total bytes do not match");
            Assert.AreEqual(downloadPipeline.Output.Data, data, "Download data does not match");
        }
    }
}
