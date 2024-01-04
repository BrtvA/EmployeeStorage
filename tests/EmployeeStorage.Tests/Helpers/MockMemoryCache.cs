using EmployeeStorage.API.Domain.EmployeeAggregate;
using EmployeeStorage.API.Infrastructure.Services;
using Microsoft.Extensions.Caching.Memory;

namespace EmployeeStorage.Tests.Helpers
{
    internal class MockMemoryCache : IMemoryCache
    {
        private Result<IEnumerable<Employee>>? _value;
        private bool _isEmpty;

        public MockMemoryCache(Result<IEnumerable<Employee>>? value)
        {
            _value = value;
            _isEmpty = value is null;
        }

        public ICacheEntry CreateEntry(object key)
        {
            return new MockCacheEntry { Key = key };
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Remove(object key)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(object key, out object? value)
        {
            value = _value;
            return !_isEmpty;
        }
    }
}
