using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace EmployeeStorage.Tests.Helpers;

public class MockCacheEntry : ICacheEntry
{
    public object Key { get; set; } = null!;

    public object? Value { get; set; }
    public DateTimeOffset? AbsoluteExpiration { get; set; }
    public TimeSpan? AbsoluteExpirationRelativeToNow { get; set; }
    public TimeSpan? SlidingExpiration { get; set; }

    public IList<IChangeToken> ExpirationTokens { get; set; } = null!;

    public IList<PostEvictionCallbackRegistration> PostEvictionCallbacks { get; set; } = null!;

    public CacheItemPriority Priority { get; set; }
    public long? Size { get; set; }

    public void Dispose()
    {

    }
}
