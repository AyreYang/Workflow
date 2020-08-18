using System.Collections.Generic;

namespace Database.Entity.Objects
{
    public class ReadOnlyPair<TK, TV>
    {
        public TK Key { get; private set; }
        public TV Value { get; private set; }

        public ReadOnlyPair(TK key, TV value)
        {
            Key = key;
            Value = value;
        }

        public KeyValuePair<TK, TV> ToKeyValuePair()
        {
            return new KeyValuePair<TK, TV>(Key, Value);
        }
    }
}
