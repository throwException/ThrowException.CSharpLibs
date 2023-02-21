using System;
using System.Collections.Generic;

namespace ThrowException.CSharpLibs.PipelineLib
{
    public class HashStage : ProcessStage
    {
        public HashStage(OpensslHash hash )
            : base("openssl-enc", Software.OpensslPath, "{0} -binary", hash.OpensslString())
        {
        }
    }
}
