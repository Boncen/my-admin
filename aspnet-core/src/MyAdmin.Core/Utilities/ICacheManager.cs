namespace MyAdmin.Core.Utilities;

public interface ICacheManager
{
    void DeleteCache(string key);
    object Save(string key, object data);
    object Save(string key, object data, TimeSpan expire);
    object SaveIfNotExist(string key, object data);
    object SaveIfNotExist(string key, object data, TimeSpan expire);
    object? Get(string key);
}

public interface ICacheManager<T> : ICacheManager
{
    T Save(string key, T data);
    T Save(string key, T data, TimeSpan expire);
    T SaveIfNotExist(string key, T data);
    T SaveIfNotExist(string key, T data, TimeSpan expire);
    new T? Get(string key);
}