using System;
using System.Collections.Generic;

namespace ThrowException.CSharpLibs.ArgumentsParserLib
{
    public class PreParser
    {
        private enum Quoted
        {
            None,
            DoubleQuote,
            SingleQuote,
        }

        public IEnumerable<string> Parse(string commandLine)
        {
            var chars = new Queue<char>(commandLine);
            var quoted = Quoted.None;
            string token = string.Empty;

            while (chars.Count > 0)
            {
                var c = chars.Dequeue();

                switch (quoted)
                {
                    case Quoted.None:
                        switch (c)
                        {
                            case '\\':
                                var n = chars.Dequeue();
                                switch (n)
                                {
                                    case '\\':
                                        token += "\\";
                                        break;
                                    case '"':
                                        token += "\"";
                                        break;
                                    case '\'':
                                        token += "'";
                                        break;
                                    default:
                                        throw new ArgumentsParseException("Illegal escape sequence");
                                }
                                break;
                            case '"':
                                quoted = Quoted.DoubleQuote;
                                break;
                            case '\'':
                                quoted = Quoted.SingleQuote;
                                break;
                            case ' ':
                            case '\t':
                                if (!string.IsNullOrEmpty(token))
                                {
                                    yield return token;
                                }
                                token = string.Empty;
                                break;
                            default:
                                token += c.ToString();
                                break;
                        }
                        break;
                    case Quoted.DoubleQuote:
                        switch (c)
                        {
                            case '\\':
                                var n = chars.Dequeue();
                                switch (n)
                                {
                                    case '\\':
                                        token += "\\";
                                        break;
                                    case '"':
                                        token += "\"";
                                        break;
                                    default:
                                        throw new ArgumentsParseException("Illegal escape sequence");
                                }
                                break;
                            case '"':
                                quoted = Quoted.None;
                                break;
                            default:
                                token += c.ToString();
                                break;
                        }
                        break;
                    case Quoted.SingleQuote:
                        switch (c)
                        {
                            case '\'':
                                quoted = Quoted.None;
                                break;
                            default:
                                token += c.ToString();
                                break;
                        }
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }

            if (!string.IsNullOrEmpty(token))
            {
                yield return token;
            }
        }
    }
}
