using System;
using System.Collections.Generic;
using System.Linq;

namespace WavePlayer.Caching
{
    internal class Cache<T> where T : class
    {
        private readonly Func<T, bool> _skipPredicate;
        private readonly object _lock = new object();
        private readonly List<T> _items = new List<T>();
        private readonly int _cachedSize;

        public Cache(int cacheSize, Func<T, bool> skipPredicate)
        {
            _cachedSize = cacheSize;
            _skipPredicate = skipPredicate;
        }

        public Cache(int cacheSize) : this(cacheSize, null)
        {
        }

        public IEnumerable<T> Items { get { return _items; } }

        public T GetItem(Func<T, bool> predicate, Func<T> createFunc)
        {
            T item;

            lock (_lock)
            {
                item = _items.SingleOrDefault(predicate);

                if (item == null && createFunc != null)
                {
                    item = createFunc();

                    SetItem(item);
                }
            }

            return item;
        }

        public void SetItem(T item)
        {
            Clear(false);

            lock (_lock)
            {
                if (!_items.Contains(item))
                {
                    _items.Add(item);
                }
            }
        }

        public void Clear()
        {
            Clear(true);
        }

        private void Clear(bool forceClear)
        {
            lock (_lock)
            {
                if (!forceClear && _items.Count <= _cachedSize)
                {
                    return;
                }

                for (var idx = 0; idx < _items.Count; idx++)
                {
                    var itemToRemove = _items[idx];

                    if (!forceClear && _skipPredicate != null && _skipPredicate(itemToRemove))
                    {
                        continue;
                    }

                    _items.Remove(itemToRemove);
                }
            }
        }
    }
}
