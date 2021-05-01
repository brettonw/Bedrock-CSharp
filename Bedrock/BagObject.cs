using System;

namespace Bedrock
{
    public class BagObject : Bag
    {
        class Pair
        {
            public String key;
            public Object value;

            public Pair(String key)
            {
                this.key = key;
            }
        }

        private Pair[] container;

        // C# has this core style inconsistency that methods and properties are capitalized, but
        // variables are lowercase, which makes for very unclear code using simple properties. it
        // would be better to use the k&r style, but c'est la vie.
        private int count;
        public int Count { get => count; }

        private void Grow(int gapIndex)
        {
            var src = container;
            if (count == container.Length)
            {
                // if the array is smaller than the cap then double its size, otherwise just add the block
                var newSize = (count > DOUBLING_CAP) ? (count + DOUBLING_CAP) : (count * 2);
                container = new Pair[newSize];
                Array.Copy(src, 0, container, 0, gapIndex);
            }
            Array.Copy(src, gapIndex, container, gapIndex + 1, count - gapIndex);
            ++count;
        }

        private int BinarySearch(String key)
        {
            // starting conditions mapped to either end of the internal store
            var low = (int) 0;
            var high = count - 1;

            // loop as long as the bounds have not crossed
            while (low <= high)
            {
                // compute the midpoint, and compare the search term against the key stored there
                var mid = (low + high) / 2;
                var cmp = String.Compare(container[mid].key, key);

                // check the result of the comparison
                if (cmp < 0)
                {
                    // the current midpoint is below the target value, set 'low' to one past it so the
                    // next loop will look only at the part of the array above the midpoint
                    low = mid + 1;
                }
                else if (cmp > 0)
                {
                    // the current midpoint is above the target value, set 'high' to one below it so the
                    // next loop will look only at the part of the array below the midpoint
                    high = mid - 1;
                }
                else
                {
                    // "Found it!" she says in a sing-song voice
                    return mid;
                }
            }
            // key not found, return an encoded version of where the key SHOULD be
            return -(low + 1);
        }

        private Pair GetOrAddPair(String key)
        {
            // conduct a binary search for where the pair should be
            var index = BinarySearch(key);
            if (index < 0)
            {
                // the binary search returns a funky encoding of the index where the new value
                // should go when it's not there, so we have to decode that number (-index + 1)
                index = -(index + 1);

                // make sure there is room in the underlying container, then store a new (empty) Pair
                Grow(index);
                container[index] = new Pair(key);
            }
            return container[index];
        }

        public BagObject(int size = UNKNOWN_SIZE)
        {
            count = 0;
            container = new Pair[Math.Max(size, DEFAULT_CONTAINER_SIZE)];
        }

        public override Object GetObject(String key)
        {
            var path = key.Split(Key.PATH_SEPARATOR, 2);
            var index = BinarySearch(path[0]);
            if (index >= 0)
            {
                // grab the found element... if the path was only one element long, this is the element
                // we were looking for, otherwise recur on the found element as another BagObject
                var pair = container[index];
                var found = pair.value;
                return (path.Length == 1) ? found : ((Bag)found).GetObject(path[1]);
            }
            return null;
        }


        public BagObject Put(String key, Object value)
        {
            // convert the element to internal storage format, and don't bother with the rest if that's
            // a null value (per the docs above)
            value = Objectify(value);
            if (value != null)
            {
                // separate the key into path components, the "local" key value is the first component,
                // so use that to conduct the search. If there is an element there, we want to get it,
                // otherwise we want to create it.
                var path = key.Split(Key.PATH_SEPARATOR, 2);
                var pair = GetOrAddPair(path[0]);
                if (path.Length == 1)
                {
                    // this was the only key in the path, so it's the end of the line, store the value
                    pair.value = value;
                }
                else
                {
                    // this is not the leaf key, so we set the pair value to be a new BagObject if
                    // necessary, then traverse via recursion,
                    var bagObject = (BagObject)pair.value;
                    if (bagObject == null)
                    {
                        pair.value = (bagObject = new BagObject());
                    }
                    bagObject.Put(path[1], value);
                }
            }
            return this;
        }

        public Object this[String key]
        {
            get { return GetObject(key); }
            set { Put(key, value); }
        }

        public static BagObject Open(String key, Object value)
        {
            return new BagObject().Put(key, value);
        }

        public BagObject Add(String key, Object value)
        {
            // separate the key into path components, the "local" key value is the first component,
            // so use that to conduct the search. If there is an element there, we want to get it,
            // otherwise we want to create it.
            var path = key.Split(Key.PATH_SEPARATOR, 2);
            var pair = GetOrAddPair(path[0]);
            if (path.Length == 1)
            {
                // this is the end of the line, so we want to store the requested object
                var bagArray = (BagArray)null;
                var found = pair.value;
                if ((value = Objectify(value)) == null)
                {
                    if (found == null)
                    {
                        // 1) object is null, key does not exist - create array
                        pair.value = (bagArray = new BagArray());
                    }
                    else if (found is BagArray) {
                        // 2) object is null, key exists (is array)
                        bagArray = (BagArray)found;
                    } else
                    {
                        // 3) object is null, key exists (is not array) - create array, store existing value
                        pair.value = (bagArray = new BagArray(2));
                        bagArray.Add(found);
                    }

                    // and store the null value in the array
                    bagArray.Add(null);
                }
                else
                {
                    if (found == null)
                    {
                        // 4) object is not null, key does not exist - store as bare value
                        pair.value = value;
                    }
                    else
                    {
                        if (found is BagArray) {
                            // 5) object is not null, key exists (is array) - add new value to array
                            bagArray = (BagArray)found;
                        } else
                        {
                            // 6) object is not null, key exists (is not array) - create array, store existing value, store new value
                            pair.value = (bagArray = new BagArray(2));
                            bagArray.Add(found);
                        }
                        bagArray.Add(value);
                    }
                }
            }
            else
            {
                // this is not the leaf key, so we set the pair value to be a new BagObject if
                // necessary, then traverse via recursion,
                var bagObject = (BagObject)pair.value;
                if (bagObject == null)
                {
                    pair.value = (bagObject = new BagObject());
                }
                bagObject.Add(path[1], value);
            }
            return this;
        }

        public BagObject Remove(String key)
        {
            var path = key.Split(Key.PATH_SEPARATOR, 2);
            var index = BinarySearch(path[0]);
            if (index >= 0)
            {
                if (path.Length == 1)
                {
                    var gapIndex = index + 1;
                    Array.Copy(container, gapIndex, container, index, count - gapIndex);
                    --count;
                }
                else
                {
                    var found = (BagObject)container[index].value;
                    found.Remove(path[1]);
                }
            }
            return this;
        }

        public Boolean Has(String key)
        {
            var path = key.Split(Key.PATH_SEPARATOR, 2);
            var index = BinarySearch(path[0]);
            try
            {
                return (index >= 0) && ((path.Length == 1) || ((BagObject)container[index].value).Has(path[1]));
            }
            catch (Exception exception)
            {
                // if a requested value is not a BagObject - this should be an exceptional case
                return false;
            }
        }

        public String[] Keys()
        {
            var keys = new String[count];
            for (int i = 0; i < count; ++i)
            {
                keys[i] = container[i].key;
            }
            return keys;
        }
    }
}
