using System;
using ThrowException.CSharpLibs.PipelineLib;

namespace ThrowException.CSharpLibs.PipelineTest
{
    public class TestConfig : IConfig
    {
        public string OpensslPath { get { return "/usr/bin/openssl"; } }

        public string GzipPath { get { return "/usr/bin/gzip"; } }

        public string StreamAuthPath { get { return "/home/user/dev/streamauth/streamauth"; } }

        public string AwsCliPath { get { return "/usr/bin/aws"; } }

        public string BlockdeltaPath { get { return "/home/user/dev/blockdelta/blockdelta"; } }
    }
}
