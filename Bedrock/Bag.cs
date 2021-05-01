using System;

namespace Bedrock
{
    public abstract class Bag
    {
        protected const int UNKNOWN_SIZE = -1;
        protected const int DEFAULT_CONTAINER_SIZE = 1;
        protected const int DOUBLING_CAP = 128;

        protected Object Objectify(Object value)
        {
            if (value != null)
            {
                var type = value.GetType();
                switch (type.FullName)
                {
                    case "System.String":
                        // is this the right place to do a transformation that converts quotes to some
                        // escape character?
                        return value;

                    case "System.Int64":
                    case "System.UInt64":
                    case "System.Int32":
                    case "System.UInt32":
                    case "System.Int16":
                    case "System.UInt16":
                    case "System.SByte":
                    case "System.Byte":
                    case "System.Char":
                    case "System.Boolean":
                    case "System.Double":
                    case "System.Single":
                        return value.ToString ();

                    case "Bedrock.BagObject":
                    case "Bedrock.BagArray":
                        return value;

                    default:
                        // if it's an enum, just get the string value
                        if (type.IsEnum)
                        {
                            return Enum.GetName(type, value);
                        }
                        // no other type should be stored in the bedrock classes
                        throw new NotSupportedException ("Unhandled type: " + type.Name);
                }
            }
            return null;
        }

        abstract public Object GetObject(String key);
    }
}
