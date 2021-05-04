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
            count = 0;
            container = new Object[Math.Max(size, DEFAULT_CONTAINER_SIZE)];
        }

        private void Grow(int gapIndex)
        {
            // save the existing container
            var src = container;

            // compute the number of values that will have to move, and from it, the new count - and
            // therefore the new size needed to include all of the elements of the array. the cases are:
            //
            // 1) the gapIndex is in the area of the array already in use, some elements will have to be
            //    moved to make room for the new element, and the array might need to be expanded to
            //    accommodate that
            //
            // 2) the gapIndex is at the end of the array already in use, no elements will have to be
            //    moved to make room for it, but the array might need to be expanded to accommodate the
            //    new element
            //
            // 3) the gapIndex is outside of the range of the array already in use, no elements will
            //    have to be moved to make room for it, but the array might need to be expanded to
            //    accommodate the new element
            var moveCount = count - gapIndex;
            count = 1 + ((moveCount > 0) ? count : gapIndex);

            // get the size of the array and resize it if necessary (copying the existing elements to
            // the new array - note that this means a sparse insertion will result in null elements in
            // the array
            var size = src.Length;
            if (count > size)
            {
                do
                {
                    // if the array is smaller than the cap then double its size, otherwise just add the block
                    size = (size > DOUBLING_CAP) ? (size + DOUBLING_CAP) : (size * 2);
                }
                while (count > size);
                container = new Object[size];
                Array.Copy(src, 0, container, 0, Math.Min(gapIndex, src.Length));
            }

            // if needed, copy elements after the gapIndex
            if (moveCount > 0)
            {
                Array.Copy(src, gapIndex, container, gapIndex + 1, moveCount);
            }
        }

        public BagArray Insert(int index, Object value)
        {
            Grow(index);
            // note that arrays can store null objects, unlike bags
            container[index] = Objectify(value);
            return this;
        }

        public BagArray Add(Object value)
        {
            return Insert(count, value);
        }

        public static BagArray Open(Object value)
        {
            return new BagArray().Add (value);
        }

        public static BagArray Concat(BagArray left, BagArray right)
        {
            var count = left.count + right.count;
            var bagArray = new BagArray(count);
            bagArray.count = count;
            Array.Copy(left.container, 0, bagArray.container, 0, left.count);
            Array.Copy(right.container, 0, bagArray.container, left.count, right.count);
            return bagArray;
        }

        public BagArray Replace(int index, Object value)
        {
            // note that arrays can store null objects, unlike bags
            container[index] = Objectify(value);
            return this;
        }

        private void RemoveIndex(int index)
        {
            // assumes index has already been checked for validity
            var gapIndex = index + 1;
            Array.Copy(container, gapIndex, container, index, count - gapIndex);
            --count;
        }

        public BagArray Remove(int index)
        {
            if ((index >= 0) && (index < count))
            {
                RemoveIndex(index);
            }
            return this;
        }

        public Object GetObject(int index)
        {
            return ((index >= 0) && (index < count)) ? container[index] : null;
        }

        public Object GetAndRemove(int index)
        {
            if ((index >= 0) && (index < count))
            {
                var value = container[index];
                RemoveIndex(index);
                return value;
            }
            return null;
        }

        public Object Pop()
        {
            return GetAndRemove(count - 1);
        }

        public Object Dequeue()
        {
            return GetAndRemove(0);
        }

        private int KeyToIndex(String key)
        {
            switch (key)
            {
                case "#first": return 0;
                case "#last": return count - 1;
                //case "#add": return count;
                default:
                    try { return Int32.Parse(key); }
                    catch (Exception) { return -1; }
            }
        }

        override public Object GetObject(String key)
        {
            // separate the key into path components, the "local" key value is the first component, so
            // use that to conduct the search. We are only interested in values that indicate the search
            // found the requested key
            var path = key.Split(Key.PATH_SEPARATOR, 2);
            var index = KeyToIndex(path[0]);
            if ((index >= 0) && (index < count))
            {
                // grab the found element... if the path was only one element long, this is the element
                // we were looking for, otherwise recur on the found element as another BagObject
                var found = container[index];
                return (path.Length == 1) ? found : ((Bag)found).GetObject(path[1]);
            }
            return null;
        }

        public String GetString(int key, String defaultValue = null)
        {
            var value = GetObject(key);
            return ((value != null) && (value is String)) ? (String)value : defaultValue;
        }

        public BagObject GetBagObject(int key)
        {
            var value = GetObject(key);
            return ((value != null) && (value is BagObject)) ? (BagObject)value : null;
        }

        public BagArray GetBagArray(int key)
        {
            var value = GetObject(key);
            return ((value != null) && (value is BagArray)) ? (BagArray)value : null;
        }

        public T Get<T>(int key, Func<T> supplier)
        {
            var value = GetString(key);
            return (value != null) ?
                    (typeof(T).IsEnum ?
                        (T)Enum.Parse(typeof(T), value) :
                        (T)typeof(T).GetMethod("Parse", new Type[] { typeof(String) }).Invoke(null, new object[] { value })) :
                    supplier();
        }

    }
}
