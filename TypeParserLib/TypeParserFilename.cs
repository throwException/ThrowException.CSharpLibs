using System;
using System.IO;

namespace ThrowException.CSharpLibs.TypeParserLib
{
    public class TypeParserFilename : TypeParser<string>
    {
        public TypeParserFilename()
        {
            Enabled = false;
        }

        public override bool CanParse(string value)
        {
            return true;
        }

        public override string Parse(string value)
        {
            if (!File.Exists(value))
                throw new ValueParseException("File {0} does not exist", value);
            return value;
        }
    }
}
