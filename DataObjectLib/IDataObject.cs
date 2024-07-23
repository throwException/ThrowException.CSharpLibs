using System;
using System.Collections.Generic;

namespace ThrowException.CSharpLibs.DataObjectLib
{
    public interface IDataObject
    {
        Guid Id { get; }

        IEnumerable<IDataField> Fields { get; }
    }
}
