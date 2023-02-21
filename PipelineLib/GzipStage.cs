using System;
using System.Collections.Generic;

namespace ThrowException.CSharpLibs.PipelineLib
{
    public class GzipStage : ProcessStage
    {
        public GzipStage(bool decompress)
            : base("gzip", Software.GzipPath, decompress ? "-d" : string.Empty)
        {
        }
    }
}
