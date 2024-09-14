
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MyAdmin.Core.Options;

namespace MyAdmin.Core.Utilities;

public class MemeroryCacheManager:ICacheManager
{
    private readonly CacheOptions? _options;
    private readonly IMemoryCache _memoryCache;
    public MemeroryCacheManager(IOptions<MaFrameworkOptions> options, IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
        _options = options.Value?.Cache;
    }

    public void DeleteCache(string key)
    {
        _memoryCache.Remove(key);
    }

    public object Save(string key, object data)
    {
       return  _memoryCache.Set(key, data);
    }

    public object Save(string key, object data, TimeSpan expire)
    {
        return _memoryCache.Set(key, data, expire);
    }

    public object SaveIfNotExist(string key, object data)
    {
        if (_memoryCache.TryGetValue(key,out object? obj))
        {
            if (obj == null)
            {
                return _memoryCache.Set(key, data);
            }

            return obj;
        }
        else
        {
            return _memoryCache.Set(key, data);
        }
    }

    public object SaveIfNotExist(string key, object data, TimeSpan expire)
    {
        if (_memoryCache.TryGetValue(key,out object? obj))
        {
            if (obj == null)
            {
                return _memoryCache.Set(key, data, expire);
            }

            return obj;
        }
        else
        {
            return _memoryCache.Set(key, data, expire);
        }
    }

    public object? Get(string key)
    {
        return _memoryCache.Get(key);
    }
}

public class MemeroryCacheManager<T> : MemeroryCacheManager, ICacheManager<T>
{
    private readonly CacheOptions? _options;
    private readonly IMemoryCache _memoryCache;
    public MemeroryCacheManager(IOptions<MaFrameworkOptions> options, IMemoryCache memoryCache) : base(options, memoryCache)
    {
        _memoryCache = memoryCache;
        _options = options.Value?.Cache;
    }

    public T Save(string key, T data)
    {
        return _memoryCache.Set<T>(key, data);
    }

    public T Save(string key, T data, TimeSpan expire)
    {
        return _memoryCache.Set<T>(key, data, expire);
    }

    public T SaveIfNotExist(string key, T data)
    {
        if (_memoryCache.TryGetValue<T>(key,out T? obj))
        {
            if (obj == null)
            {
                return _memoryCache.Set<T>(key, data);
            }

            return obj;
        }
        else
        {
            return _memoryCache.Set<T>(key, data);
        }
    }

    public T SaveIfNotExist(string key, T data, TimeSpan expire)
    {
        if (_memoryCache.TryGetValue<T>(key,out T? obj))
        {
            if (obj == null)
            {
                return _memoryCache.Set<T>(key, data, expire);
            }

            return obj;
        }
        else
        {
            return _memoryCache.Set<T>(key, data, expire);
        }
    }

    public new T? Get(string key)
    {
        return _memoryCache.Get<T>(key);
    }
}