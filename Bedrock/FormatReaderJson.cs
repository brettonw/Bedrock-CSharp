using System;
namespace Bedrock
{
    public class FormatReaderJson : FormatReaderParsed
    {

        public FormatReaderJson() { }

        public FormatReaderJson(String input) : base(input)
        {
        }

        public BagArray ReadBagArray()
        {
            // <Array> :: [ ] | [ <Elements> ]
            var bagArray = new BagArray();
            return (Expect('[') && ReadElements(bagArray) && Require(']')) ? bagArray : null;
        }

        private bool StoreValue(BagArray bagArray)
        {
            // the goal here is to try to read a "value" from the input stream, and store it into the
            // BagArray. BagArrays can store null values, so we have a special handling case to make
            // sure we properly convert "null" string to null value - as distinguished from a failed
            // read, which returns null value to start.the method returns true if a valid value was
            // fetched from the stream (in which case it was added to the BagArray)
            var value = ReadValue();
            if (value != null)
            {
                // special case for "null"
                if ((value is String) && (((String)value) == "null"))
                {
                    value = null;
                }
                bagArray.Add(value);
                return true;
            }
            return false;
        }

        private bool ReadElements(BagArray bagArray)
        {
            // <Elements> ::= <Value> | <Value> , <Elements>
            var result = true;
            if (StoreValue(bagArray))
            {
                while (Expect(','))
                {
                    result = Require(StoreValue(bagArray), "Valid value");
                }
            }
            return result;
        }

        public BagObject ReadBagObject()
        {
            // <Object> ::= { } | { <Members> }
            var bagObject = new BagObject();
            return (Expect('{') && ReadMembers(bagObject) && Require(Expect('}'), "Valid pair (<String>:<Value>) or '}'")) ? bagObject : null;
        }

        private bool ReadMembers(BagObject bagObject)
        {
            // <Members> ::= <Pair> | <Pair> , <Members>
            var result = true;
            if (ReadPair(bagObject))
            {
                while (Expect(','))
                {
                    result = Require(ReadPair(bagObject), "Valid pair (<String>:<Value>)");
                }
            }
            return result;
        }

        private bool StoreValue(BagObject bagObject, String key)
        {
            // the goal here is to try to read a "value" from the input stream, and store it into the
            // BagObject. BagObject can NOT store null values, so we have a special handling case to
            // make sure we properly convert "null" string to null value - as distinguished from a failed
            // read, which returns null value to start. the method returns true if a valid value was
            // fetched from the stream, regardless of whether a null value was stored in the BagObject.
            var value = ReadValue();
            if (value != null)
            {
                // special case for "null"
                if (!((value is String) && (((String)value) == "null")))
                {
                    bagObject.Put(key, value);
                }
                return true;
            }
            return false;
        }

        private bool ReadPair(BagObject bagObject)
        {
            // <Pair> ::= <String> : <Value>
            var key = ReadString();
            return (key != null) && (key.Length > 0) && Require(':') && Require(StoreValue(bagObject, key), "Valid value");
        }

        private static char[] SortString(String str)
        {
            var chars = str.ToCharArray();
            Array.Sort(chars);
            return chars;
        }

        private static char[] BARE_VALUE_STOP_CHARS = SortString(" \u00a0\t\n:{}[]\",");
        private static char[] QUOTED_STRING_STOP_CHARS = SortString("\n\"");

        private bool NotIn(char[] stopChars, char c)
        {
            int i = 0;
            int end = stopChars.Length;
            char stopChar = (char)0;
            while ((i < end) && (c > (stopChar = stopChars[i])))
            {
                ++i;
            }
            return stopChar != c;
        }

        private int ConsumeUntilStop(char[] stopChars)
        {
            var start = index;
            char c;
            //while (check () && (Arrays.binarySearch (stopChars, c = input.charAt (index)) < 0)) {
            while (Check() && NotIn(stopChars, (c = input[index])))
            {
                // using the escape mechanism is like a free pass for the next character, but we
                // don't do any transformation on the substring, just return it as written
                index += (c == '\\') ? 2 : 1;
            }
            return start;
        }

        private String ReadString()
        {
            // " chars " | <chars>
            var result = (String)null;
            if (Expect('"'))
            {
                // digest the string, and be sure to eat the end quote
                var start = ConsumeUntilStop(QUOTED_STRING_STOP_CHARS);
                result = input.SubStr(start, index++);
            }
            return result;
        }

        private String ReadBareValue()
        {
            // " chars " | <chars>
            var result = (String)null;

            // technically, we're being sloppy allowing bare values in some cases where quoted strings
            // are the standard, but it's part of the simplified structure we support. This allows us to
            // read valid JSON files without handling every single pedantic case.
            var start = ConsumeUntilStop(BARE_VALUE_STOP_CHARS);

            // capture the result if we actually consumed some characters
            if (index > start)
            {
                result = input.SubStr(start, index);
            }

            return result;
        }

        private Object ReadValue()
        {
            // <Value> ::= <String> | <Object> | <Array>
            ConsumeWhiteSpace();
            if (Check())
            {
                switch (input[index])
                {
                    case '{': return ReadBagObject();
                    case '[': return ReadBagArray();
                    case '"': return ReadString();
                    default: return ReadBareValue();
                }
            }
            return null;
        }
    }
}
