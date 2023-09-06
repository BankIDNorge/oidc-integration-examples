using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Caching.Memory;

namespace ExampleAspnet.Storage;

public interface ICache : IDisposable
{
    /// <summary>
    /// Inserts a value to the cache storage associated by its cache key, with an expiration time.
    /// </summary>
    /// <param name="key">The cache key, this is later used to retrieve the cached data.</param>
    /// <param name="value">The value to cache.</param>
    /// <param name="expirationTime">The expiration time for the cache value.</param>
    /// <typeparam name="T">The type of the value.</typeparam>
    void AddData<T>(string key, T value, DateTime expirationTime);

    /// <summary>
    /// Inserts a JWT to the cache storage associated by its cache key, with an expiration time inferred from the JWT.
    /// </summary>
    /// <param name="key">The cache key, this is later used to retrieve the cached data.</param>
    /// <param name="value">The JWT to cache.</param>
    void AddToken(string key, string value);

    /// <summary>
    /// Retrieves a value from the cache storage associated by its cache key.
    /// </summary>
    /// <param name="key">The cache key.</param>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <returns>The cached value, if exists. Otherwise, null.</returns>
    T? GetData<T>(string key);
}

/// <summary>
/// A simple in-memory cache backed by <see cref="MemoryCache"/>
/// </summary>
public class Cache : ICache
{
    private readonly MemoryCache _cache = new(new MemoryCacheOptions());

    public void AddData<T>(string key, T value, DateTime expirationTime)
    {
        _cache.Set(key, value, expirationTime);
    }

    public void AddToken(string key, string value)
    {
        var exp = GetJwtExpiryDate(value)?.Subtract(TimeSpan.FromMinutes(1));
        if (exp == null) throw new InvalidOperationException("The provided JWT does not contain an expiry date.");

        _cache.Set(key, value, exp.Value);
    }

    public T? GetData<T>(string key)
    {
        return _cache.Get<T>(key);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _cache.Dispose();
    }

    /// <summary>
    /// Retrieves the expiry date and time of a JWT token.
    /// </summary>
    /// <param name="token">The JWT token to parse.</param>
    /// <returns>A <see cref="DateTime"/> value representing the JWT's expiration time.</returns>
    private static DateTime? GetJwtExpiryDate(string token)
    {
        JwtSecurityToken jwtSecurityToken;
        try
        {
            jwtSecurityToken = new JwtSecurityToken(token);
        }
        catch (Exception)
        {
            return null;
        }

        return jwtSecurityToken.ValidTo;
    }
}