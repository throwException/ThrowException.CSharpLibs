using System;
using System.Collections.Generic;

namespace ThrowException.CSharpLibs.PipelineLib
{
    public class EncryptStage : ProcessStage
    {
        private const string PasswordEnvironmentVariable = "OPENSSL_ENCRYPTION_PASSWORD";

        private static string ConstructArguments(OpensslCipher cipher, bool decrypt)
        {
            return string.Format(
                "{0} {1} -pbkdf2 -pass env:{2} ", 
                cipher.OpensslString(),
                decrypt ? "-d" : "-e",
                PasswordEnvironmentVariable);
        }

        public EncryptStage(IConfig config, OpensslCipher cipher, string key, bool decrypt)
            : base("openssl-enc", config.OpensslPath, ConstructArguments(cipher, decrypt))
        {
            WithEnvironment(PasswordEnvironmentVariable, key);
        }
    }
}
