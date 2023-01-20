using System;

namespace ThrowException.CSharpLibs.ArgumentsParserLib
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class VerbAttribute : Attribute
    {
        public string Verb { get; private set; }
        public bool Default { get; private set; }

        public VerbAttribute(string verb, bool defaultVerb)
        {
            Verb = verb;
            Default = defaultVerb;
        }

        public VerbAttribute(string verb)
        {
            Verb = verb;
            Default = false;
        }

        public VerbAttribute()
        {
            Verb = null;
            Default = true;
        }
    }
}
