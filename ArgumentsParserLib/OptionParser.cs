using System;
using System.Collections.Generic;

namespace ThrowException.CSharpLibs.ArgumentsParserLib
{
    public class OptionParser
    {
        public ArgumentInstance Parse(IEnumerable<string> tokens)
        {
            OptionInstance currentOption = null;
            var result = new ArgumentInstance();
            bool endOfOptions = false;

            foreach (var token in tokens)
            {
                if (!endOfOptions && token == "--")
                {
                    currentOption = null;
                    endOfOptions = true;
                }
                else if (!endOfOptions && token.StartsWith("--", StringComparison.Ordinal))
                {
                    var name = token.Substring(2);
                    currentOption = new OptionInstance(name);
                    result.Add(currentOption);
                }
                else if (!endOfOptions && token.StartsWith("-", StringComparison.Ordinal))
                {
                    foreach (char c in token.Substring(1))
                    {
                        currentOption = new OptionInstance(c);
                        result.Add(currentOption);
                    }
                }
                else if (currentOption != null)
                {
                    currentOption.Add(token);
                }
                else
                {
                    result.Add(token);
                }
            }

            return result;
        }
    }
}
