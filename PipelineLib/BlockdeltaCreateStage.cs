using System;
using System.Collections.Generic;

namespace ThrowException.CSharpLibs.PipelineLib
{
    public class BlockdeltaCreateStage : ProcessStage
    {
        private static string ConstructArguments(string blockSource, string blockDestination)
        {
            return string.Format(
                "create {0} {1}", 
                blockSource,
                blockDestination);
        }

        public BlockdeltaCreateStage(string blockSource, string blockDestination)
            : base("blockdelta-create", Software.BlockdeltaPath, ConstructArguments(blockSource, blockDestination))
        {
        }
    }
}
