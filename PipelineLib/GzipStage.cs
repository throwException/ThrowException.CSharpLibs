using System;
using System.Collections.Generic;

namespace ThrowException.CSharpLibs.PipelineLib
{
    public class GzipStage : ProcessStage
    {
        public GzipStage(IConfig config, bool decompress)
            : base("gzip", config.GzipPath, decompress ? "-d" : string.Empty)
        {
        }
    }
}
