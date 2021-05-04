using System;
using System.Text;

namespace Bedrock
{
    public class Key
    {
        public static String PATH_SEPARATOR = "/";

        public Key()
        {
        }

        public static String Cat(params Object[] components)
        {
            var stringBuilder = new StringBuilder();
            var separator = "";
            foreach (var component in components)
            {
                stringBuilder.Append(separator).Append(component.ToString());
                separator = PATH_SEPARATOR;
            }
            return stringBuilder.ToString();
        }
    }
}