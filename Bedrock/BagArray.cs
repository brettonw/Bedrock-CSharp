using System;
namespace Bedrock
{
    public class BagArray : Bag
    {
        private Object[] container;

        // C# has this core style inconsistency that methods and properties are capitalized, but
        // variables are lowercase, which makes for very unclear code using simple properties. it
        // would be better to use the k&r style, but c'est la vie.
        private int count;
        public int Count { get => count; }


        public BagArray(int size = UNKNOWN_SIZE)
        {
        }

        public override Object GetObject(String key)
        {
            throw new NotImplementedException();
        }

        public BagArray Add (Object value)
        {
            return this;
        }
    }
}
