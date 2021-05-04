using System;
namespace Bedrock
{
    public static class Extensions
    {
        public static String SubStr (this String str, int startIndex, int endIndex)
        {
            return str.Substring(startIndex, endIndex - startIndex);
        }
    }
}
