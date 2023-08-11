using System;
using System.Collections.Generic;

namespace ThrowException.CSharpLibs.PipelineLib
{
    public class BlockdeltaApplyStage : ProcessStage
    {
        private static string ConstructArguments(string block)
        {
            return string.Format(
                "apply {0}", 
                block);
        }

        public BlockdeltaApplyStage(IConfig config, string block)
            : base("blockdelta-create", config.BlockdeltaPath, ConstructArguments(block))
        {
        }
    }
}
