using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Domain.Core.Exceptions;

namespace Domain.Core.Caches
{
    public class CompiledCodeCache : ICache<byte[]>
    {
        public int Limit { get; set; } = 5;
        private readonly Dictionary<string, byte[]> _cache = new();
        public int Size => _cache.Count;

        public void Cache(string key, byte[] value)
        {
            var hash = GetHashString(key);
            
            if (_cache.ContainsKey(hash))
                return;

            if (Size >= Limit && !_cache.ContainsKey(hash))
                throw new CacheOverflowException(
            $"CompiledCodeCache has been overflowed! Max limit of elements is {Limit}");

            _cache.Add(hash, value);
        }

        public bool HasKey(string key) => _cache.ContainsKey(GetHashString(key));

        public byte[] GetValue(string key) => _cache[GetHashString(key)];

        public void Clean() => _ = _cache.Keys.Select(k => _cache.Remove(k));

        public void Clean(int count) => _ = _cache.Keys.Select((e, i) => i < count && _cache.Remove(e));
        
        private byte[] GetHash(string inputString)
            => SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(inputString));

        private string GetHashString(string inputString)
            => string.Join("", GetHash(inputString).Select(b => b.ToString("X2")));
    }
}