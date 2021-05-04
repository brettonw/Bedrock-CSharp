using System;
namespace Bedrock
{
    abstract public class FormatWriter
    {
        protected static String[] QUOTES = { "\"" };

        protected String Enclose(String input, String[] bracket)
        {
            var bracket0 = bracket[0];
            var bracket1 = (bracket.Length > 1) ? bracket[1] : bracket0;
            return bracket0 + input + bracket1;
        }

        protected String Quote(String input)
        {
            return Enclose(input, QUOTES);
        }

        abstract public String Write(Object value);
    }
}
