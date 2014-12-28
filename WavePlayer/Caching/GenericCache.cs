using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;

namespace WavePlayer.Caching
{
    internal sealed class GenericCache<TItem> : IDisposable where TItem : class
    {
        private readonly object _lock = new object();
        private readonly Func<TItem, bool> _keepAlive;
        private MemoryCache _memoryCache;

        public GenericCache(string name, Func<TItem, bool> keepAlive) : this(name)
        {
            _keepAlive = keepAlive;
        }

        public GenericCache(string name)
        {
            _memoryCache = new MemoryCache(name);
        }

        public TimeSpan SlidingExpiration { get; set; }

        public ICollection<TItem> Items
        {
            get
            {
                return _memoryCache.Select(e => e.Value).OfType<TItem>().ToArray();
            }
        }

        public void Set(string key, TItem item)
        {
            var existingItem = Get(key);

            if (existingItem != null &&
                item == existingItem)
            {
                return;
            }

            lock (_lock)
            {
                SetIternal(key, item);
            }
        }

        public TItem GetOrAdd(string key, Func<TItem> createItem)
        {
            if (createItem == null)
            {
                throw new ArgumentNullException("createItem");
            }

            lock (_lock)
            {
                var item = GetIternal(key);

                if (item != null)
                {
                    return (TItem)item;
                }

                var newItem = createItem();

                SetIternal(key, newItem);

                return newItem;
            }
        }

        public TItem Get(string key)
        {
            lock (_lock)
            {
                var item = GetIternal(key);

                return item as TItem;
            }
        }

        public void Clean()
        {
            lock (_lock)
            {
                var keys = _memoryCache.Select(e => e.Key).ToArray();

                foreach (var key in keys)
                {
                    _memoryCache.Remove(key);
                }
            }
        }

        public void Dispose()
        {
            if (_memoryCache != null)
            {
                _memoryCache.Dispose();
                _memoryCache = null;
            }
        }

        private void SetIternal(string key, TItem item)
        {
            _memoryCache.Set(key, item, CreateCacheItemPolicy());
        }

        private object GetIternal(string key)
        {
            return _memoryCache.Get(key);
        }

        private CacheItemPolicy CreateCacheItemPolicy()
        {
            return new CacheItemPolicy()
            {
                UpdateCallback = UpdateCallback,
                SlidingExpiration = SlidingExpiration
            };
        }

        private void UpdateCallback(CacheEntryUpdateArguments arguments)
        {
            var entry = _memoryCache.Get(arguments.Key);

            if (entry != null)
            {
                var item = (TItem)entry;

                if (_keepAlive != null && _keepAlive(item))
                {
                    arguments.UpdatedCacheItem = new CacheItem(arguments.Key, item);
                    arguments.UpdatedCacheItemPolicy = CreateCacheItemPolicy();
                    return;
                }
            }

            _memoryCache.Remove(arguments.Key);
        }
    }
}
