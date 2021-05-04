using System;
namespace Bedrock
{
    public class FormatReaderParsed : FormatReader
    {
        protected int index;
        protected int inputLength;
        protected int lineNumber;
        protected int lastLineIndex;
        protected bool error;

        protected FormatReaderParsed() { }

        public FormatReaderParsed(String input) :  base (input)
        {
            inputLength = (input != null) ? input.Length : 0;
            index = 0;
            lineNumber = 1;
            lastLineIndex = 0;
        }

        protected bool Check()
        {
            return (!error) && (index < inputLength);
        }

        protected void ConsumeWhiteSpace()
        {
            // consume white space (space, carriage return, tab, etc.
            while (Check())
            {
                switch (input[index])
                {
                    // tab, space, nbsp
                    case '\t':
                    case ' ':
                    case '\u00a0':
                        ++index;
                        break;
                    // carriage return - the file reader converts all returns to \n
                    case '\n':
                        ++index;
                        ++lineNumber;
                        lastLineIndex = index;
                        break;
                    default:
                        return;
                }
            }
        }

        protected bool Expect(char c)
        {
            ConsumeWhiteSpace();

            // the next character should be the one we expect
            if (Check() && (input[index] == c))
            {
                ++index;
                return true;
            }
            return false;
        }

        protected bool Require(bool condition, String explanation)
        {
            if (!condition)
            {
                OnReadError(explanation + " REQUIRED");
            }
            return condition;
        }

        protected bool Require(char c)
        {
            return Require(Expect(c), "'" + c + "'");
        }

        protected void OnReadError(String errorMessage)
        {

            // log the messages, we only need to output the line if this is the first time the error is
            // being reported
            if (!error)
            {
                // say where the error is
                Console.WriteLine("Error while parsing input on line " + lineNumber + ", near: ");
                // find the end of the current line. note: line endings could only be '\n' because the
                // input reader consumed the actual line endings for us and replaced them with '\n'
                var lineEnd = index;
                while ((lineEnd < inputLength) && (input[lineEnd] != '\n'))
                {
                    ++lineEnd;
                }
                Console.WriteLine(input.SubStr (lastLineIndex, lineEnd));

                // build the error message, by computing a carat line, and adding the error message to it
                var errorIndex = index - lastLineIndex;
                var caratChars = new char[errorIndex + 2];
                Array.Fill(caratChars, ' ');
                caratChars[errorIndex] = '^';
                var carat = new String(caratChars) + errorMessage;

                Console.WriteLine(carat);

                // set the error state
                error = true;
            }
        }
    }
}
