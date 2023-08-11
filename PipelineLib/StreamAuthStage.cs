using System;
using System.Collections.Generic;

namespace ThrowException.CSharpLibs.PipelineLib
{
    public class StreamAuthStage : ProcessStage
    {
        private const string KeyEnvironmentVariable = "STREAMAUTH_KEY";

        private static string ConstructArguments(bool verify)
        {
            return string.Format(
                "{0} {1}", 
                verify ? "verify" : "add",
                KeyEnvironmentVariable);
        }

        public StreamAuthStage(IConfig config, string key, bool verify)
            : base("streamauth", config.StreamAuthPath, ConstructArguments(verify))
        {
            WithEnvironment(KeyEnvironmentVariable, key);
        }
    }
}
