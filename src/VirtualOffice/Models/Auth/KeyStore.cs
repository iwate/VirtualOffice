using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace VirtualOffice.Models
{
    public class KeyStore
    {
        public static Func<DateTimeOffset> NowFactory = () => DateTimeOffset.UtcNow;
        private static readonly ConcurrentDictionary<string, Key> _keys = new ConcurrentDictionary<string, Key>();
        public IEnumerable<Key> Keys => _keys.Values;

        public Key CreateNew(DateTimeOffset expiresUtc)
        {
            var key = new Key
            {
                Code = Guid.NewGuid().ToString(),
                ExpiresUtc = expiresUtc
            };

            return _keys.TryAdd(key.Code, key) ? key : null;
        }

        public void DeleteOld()
        {
            var now = NowFactory.Invoke();
            var removes = Keys.Where(key => key.ExpiresUtc < now).ToList();
            foreach (var key in removes)
            {
                _keys.TryRemove(key.Code, out _);
            }
        }

        public void Delete(Key key)
        {
            _keys.TryRemove(key.Code, out _);
        }

        public bool Validate(string code)
        {
            if (string.IsNullOrEmpty(code))
                return false;

            return _keys.TryGetValue(code, out var key) ? key.ExpiresUtc >= NowFactory.Invoke() : false;
        }
    }
}
