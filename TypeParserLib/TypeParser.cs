using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ThrowException.CSharpLibs.TypeParserLib
{
    public abstract class TypeParser
    {
        public int Precedence { get; set; } = 0;

        public bool Enabled { get; set; } = true;

        public abstract Type ParsedType { get; }

        public virtual void Reset()
        {
            Enabled = true;
            Precedence = 0;
        }
    }

    public abstract class TypeParser<T> : TypeParser
    {
        public abstract string Format(T value);

        public abstract T Parse(string value);

        public abstract bool CanParse(string value);

        public override Type ParsedType { get { return typeof(T); } }
    }
}
