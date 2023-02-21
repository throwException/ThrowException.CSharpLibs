using System;

namespace ThrowException.CSharpLibs.PipelineLib
{
    public enum OpensslCipher
    {
        AES256CTR,
        Camellia256CTR,
        Chacha20,
    }

    public static class OpensslCipherExtensions
    {
        public static string OpensslString(this OpensslCipher cipher)
        { 
            switch (cipher)
            {
                case OpensslCipher.AES256CTR:
                    return "aes-256-ctr";
                case OpensslCipher.Camellia256CTR:
                    return "camellia-256-ctr";
                case OpensslCipher.Chacha20:
                    return "chacha20";
                default:
                    throw new NotSupportedException(cipher.ToString() + " not supported");
            }
        }
    }
}
