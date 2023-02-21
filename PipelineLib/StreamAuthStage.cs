using System;
using System.Collections.Generic;

namespace ThrowException.CSharpLibs.PipelineLib
{
    public class StreamAuthStage : ProcessStage
    {
        private const string KeyEnvironmentVariable = "STREAMAUTH_KEY";
        private const string IvEnvironmentVariable = "STREAMAUTH_IV";

        private static string ConstructArguments(bool verify)
        {
            return string.Format(
                "{0} {1} {2}", 
                verify ? "verify" : "add",
                KeyEnvironmentVariable,
                IvEnvironmentVariable);
        }

        public StreamAuthStage(string key, string iv, bool verify)
            : base("streamauth", Software.StreamAuthPath, ConstructArguments(verify))
        {
            WithEnvironment(KeyEnvironmentVariable, key);
            WithEnvironment(IvEnvironmentVariable, iv);
        }
    }
}
