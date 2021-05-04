using System;
using System.Text;

namespace Bedrock
{
    public class FormatWriterJson : FormatWriter
    {
        private static String[] CURLY_BRACKETS = { "{", "}" };
        private static String[] SQUARE_BRACKETS = { "[", "]" };

        public FormatWriterJson() { }

        override public String Write(Object value)
        {
            if (value != null)
            {
                switch (value.GetType().FullName)
                {
                    case "System.String": return Quote((String)value);
                    case "Bedrock.BagObject":
                        return WriteBagObject((BagObject)value);
                    case "Bedrock.BagArray":
                        return WriteBagArray((BagArray)value);

                        // we omit the default case, because there should not be any other types stored in
                        // the Bag classes - as in, they would not make it into the container, as the
                        // "Objectify" method will gate that
                }
            }
            // if we stored a null, we need to emit it as a value. This will only happen in the
            // array types, and is handled on the parsing side with a special case for reading
            // the bare value 'null' (not quoted)
            return "null";
        }

        public String WriteBagObject(BagObject bagObject)
        {
            var stringBuilder = new StringBuilder();
            var separator = "";
            foreach (var key in bagObject.Keys())
            {
                stringBuilder
                        .Append(separator)
                        .Append(Quote(key))
                        .Append(":")
                        .Append(Write(bagObject.GetObject(key)));
                separator = ",";
            }
            return Enclose(stringBuilder.ToString(), CURLY_BRACKETS);
        }

        public String WriteBagArray(BagArray bagArray)
        {
            var stringBuilder = new StringBuilder();
            var separator = "";
            for (int i = 0, end = bagArray.Count; i < end; ++i)
            {
                stringBuilder
                        .Append(separator)
                        .Append(Write(bagArray.GetObject(i)));
                separator = ",";
            }
            return Enclose(stringBuilder.ToString(), SQUARE_BRACKETS);
        }
    }
}
