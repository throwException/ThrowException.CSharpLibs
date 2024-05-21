using System;

namespace ThrowException.CSharpLibs.DatabaseObjectLib
{
    public class Meta : DatabaseObject
    {
        public Int32Field Version { get; set; }

        public Meta()
            : base()
        {
        }

        public Meta(Guid id, bool newObject = true)
            : base(id, newObject)
        {
        }
    }
}