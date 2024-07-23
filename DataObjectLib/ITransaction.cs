using System;

namespace ThrowException.CSharpLibs.DataObjectLib
{
    public interface ITransaction : IDisposable
    {
        void Commit();
        void Rollback();
    }
}
