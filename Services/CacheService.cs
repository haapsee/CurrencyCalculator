using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyConverter.Services {
    public class CacheItem<T> {
        public T Value { get; set; }
        public DateTime ExpirationTime { get; set; }
    }

    public interface ICacheService {
        void Add(string key, object value);
        bool TryGetValue(string key, out object value);
        void Remove(string key);
    }

    public class CacheService : ICacheService {
        private readonly Dictionary<string, CacheItem<object>> _cache = new Dictionary<string, CacheItem<object>>();
        private const int DefaultExpirationTime = 60 * 60; // 1 hour in seconds

        public void Add(string key, object value) {
            var expirationTime = DateTime.UtcNow.AddSeconds(DefaultExpirationTime);
            _cache[key] = new CacheItem<object> { Value = value, ExpirationTime = expirationTime };
        }

        public bool TryGetValue(string key, out object value) {
            if (!_cache.TryGetValue(key, out var cacheItem)) {
                value = null;
                return false;
            }
            if (cacheItem.ExpirationTime < DateTime.UtcNow) {
                _cache.Remove(key);
                value = null;
                return false;
            }
            value = cacheItem.Value;
            return true;
        }

        // implement CacheService.SaveConversionRate(from, to, exchangeRate);
        public void SaveConversionRate(string from, string to, decimal exchangeRate) {
            var cacheKey = $"{from}_{to}_conversion_rate";
            Add(cacheKey, exchangeRate);
        }

        // implement CacheService.GetConversionRate(from, to);'
        public decimal GetConversionRate(string from, string to) {
            var cacheKey = $"{from}_{to}_conversion_rate";
            if (TryGetValue(cacheKey, out var value)) {
                return (decimal)value;
            }
            return 0; // or throw an exception if you want to handle missing data gracefully
        }

        public void Remove(string key) {
            _cache.Remove(key);
        }
    }
}
