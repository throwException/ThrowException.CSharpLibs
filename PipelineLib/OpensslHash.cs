using System;

namespace ThrowException.CSharpLibs.PipelineLib
{
    public enum OpensslHash
    {
        SHA256,
        SHA512,
        SHA512L256,
    }

    public static class OpensslHashExtensions
    {
        public static string OpensslString(this OpensslHash hash)
        { 
            switch (hash)
            {
                case OpensslHash.SHA256:
                    return "sha256";
                case OpensslHash.SHA512:
                    return "sha512";
                case OpensslHash.SHA512L256:
                    return "sha512-256";
                default:
                    throw new NotSupportedException(hash.ToString() + " not supported");
            }
        }
    }
}
