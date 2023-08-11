using System;
using System.Collections.Generic;

namespace ThrowException.CSharpLibs.PipelineLib
{
    public class HashStage : ProcessStage
    {
        public HashStage(IConfig config, OpensslHash hash)
            : base("openssl-enc", config.OpensslPath, "{0} -binary", hash.OpensslString())
        {
        }
    }
}
