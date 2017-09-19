using System;
using System.Collections.Concurrent;

namespace AutoCopyLib
{
    internal class LockingConcurrentDictionary<TKey,TValue>
    {
        private readonly ConcurrentDictionary<TKey, Lazy<TValue>> _dictionary;

        private readonly Func<TKey, Lazy<TValue>> _valueFactory;

        public LockingConcurrentDictionary(Func<TKey, TValue> valueFactory)
        {
            this._dictionary = new ConcurrentDictionary<TKey, Lazy<TValue>>();
            this._valueFactory = ((TKey key) => new Lazy<TValue>(() => valueFactory(key)));
        }

        public TValue GetOrAdd(TKey key)
        {
            return this._dictionary.GetOrAdd(key, this._valueFactory).Value;
        }
    }
}
