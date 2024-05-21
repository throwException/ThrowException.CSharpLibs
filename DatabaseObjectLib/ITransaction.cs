using System;

namespace ThrowException.CSharpLibs.DatabaseObjectLib
{
    public interface ITransaction : IDisposable
    {
        void Commit();
        void Rollback();
    }
}
