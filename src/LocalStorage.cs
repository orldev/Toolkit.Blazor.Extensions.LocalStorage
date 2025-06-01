using System.Text.Json;
using Microsoft.JSInterop;
using Toolkit.Cryptography.Interfaces;

namespace Toolkit.Blazor.Extensions.LocalStorage;

/// <summary>
/// Provides a secure implementation of <see cref="ILocalStorage"/> that interacts with the browser's localStorage API
/// through JavaScript interop, with added encryption for stored values.
/// </summary>
/// <remarks>
/// <para>
/// This class wraps the browser's localStorage API (window.localStorage) which persists data beyond page refreshes
/// and browser sessions. All stored values are automatically encrypted using the provided <see cref="ISymmetricCipher"/>.
/// </para>
/// <para>
/// Browser Compatibility: Works in all modern browsers that support the Web Storage API.
/// Storage Limit: Typically 5MB per origin (protocol + domain + port).
/// Data Persistence: Survives browser restarts and system reboots.
/// </para>
/// <para>
/// Security Note: While values are encrypted, keys remain visible. Sensitive information should not be used as keys.
/// </para>
/// </remarks>
/// <param name="jSRuntime">The JavaScript runtime for interop calls</param>
/// <param name="cryptography">The symmetric cipher for encrypting/decrypting stored values</param>
public class LocalStorage(IJSRuntime jSRuntime, ISymmetricCipher cryptography) : ILocalStorage
{
    /// <summary>
    /// Removes the item with the specified key from the browser's localStorage.
    /// </summary>
    /// <param name="key">The key of the item to remove</param>
    /// <param name="ct">Optional cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    /// <remarks>
    /// This directly calls localStorage.removeItem() via JS interop.
    /// No error is thrown if the key doesn't exist.
    /// </remarks>
    public async Task RemoveAsync(string key, CancellationToken ct = default)
    {
        await jSRuntime
            .InvokeVoidAsync("localStorage.removeItem", ct, key)
            .ConfigureAwait(false);
    }
    
    /// <summary>
    /// Stores a string value in localStorage after encrypting it.
    /// </summary>
    /// <param name="key">The storage key</param>
    /// <param name="value">The string value to store</param>
    /// <param name="ct">Optional cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    /// <remarks>
    /// The value is encrypted using <see cref="ISymmetricCipher"/> before storage.
    /// Equivalent to localStorage.setItem() in JavaScript.
    /// </remarks>
    public async Task SaveStringAsync(string key, string value, CancellationToken ct = default)
    { 
        var encryptBase64 = await cryptography.EncryptToBase64Async(value);
        
        await jSRuntime
            .InvokeVoidAsync("localStorage.setItem", ct, key, encryptBase64)
            .ConfigureAwait(false);
    }
    
    /// <summary>
    /// Retrieves and decrypts a string value from localStorage.
    /// </summary>
    /// <param name="key">The storage key</param>
    /// <param name="ct">Optional cancellation token</param>
    /// <returns>
    /// The decrypted string value, or null if the key doesn't exist or the stored value is empty
    /// </returns>
    /// <remarks>
    /// Returns null if the key doesn't exist (consistent with localStorage.getItem() behavior).
    /// Throws cryptographic exceptions if decryption fails.
    /// </remarks>
    public async ValueTask<string?> GetStringAsync(string key, CancellationToken ct = default)
    {
        var encryptBase64 = await jSRuntime
            .InvokeAsync<string?>("localStorage.getItem", ct, key)
            .ConfigureAwait(false);
        
        if (string.IsNullOrEmpty(encryptBase64))
            return null;
        
        return await cryptography.DecryptFromBase64Async(encryptBase64);
    }
    
    /// <summary>
    /// Serializes and stores an object in localStorage after encrypting it.
    /// </summary>
    /// <typeparam name="T">The type of object being stored</typeparam>
    /// <param name="key">The storage key</param>
    /// <param name="value">The object to store</param>
    /// <param name="options">Optional JSON serialization settings</param>
    /// <param name="ct">Optional cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    /// <remarks>
    /// The object is serialized to JSON and then encrypted before storage.
    /// Uses System.Text.Json for serialization.
    /// </remarks>
    public async Task SaveObjectAsync<T>(string key, T value, JsonSerializerOptions? options = null, CancellationToken ct = default)
    {
        var serialize = JsonSerializer.Serialize(value, options);
        
        await SaveStringAsync(key, serialize, ct);
    }
    
    /// <summary>
    /// Retrieves and deserializes an object from localStorage.
    /// </summary>
    /// <typeparam name="T">The target deserialization type</typeparam>
    /// <param name="key">The storage key</param>
    /// <param name="ct">Optional cancellation token</param>
    /// <returns>
    /// The deserialized object, or default(T) if the key doesn't exist or the value is empty
    /// </returns>
    /// <remarks>
    /// The stored value is first decrypted before JSON deserialization.
    /// Returns default(T) (typically null) for non-existent keys rather than throwing.
    /// </remarks>
    public async ValueTask<T?> GetObjectAsync<T>(string key, CancellationToken ct = default)
    {
        var value = await GetStringAsync(key, ct);
        
        return string.IsNullOrEmpty(value) ? default : JsonSerializer.Deserialize<T>(value);
    }

    /// <summary>
    /// Checks if a key exists in localStorage.
    /// </summary>
    /// <param name="key">The key to check</param>
    /// <param name="ct">Optional cancellation token</param>
    /// <returns>True if the key exists, false otherwise</returns>
    /// <remarks>
    /// Uses JavaScript's hasOwnProperty check on the localStorage object.
    /// More efficient than checking for null after getItem().
    /// </remarks>
    public async ValueTask<bool> ContainKeyAsync(string key, CancellationToken ct = default)
    {
        return await jSRuntime
            .InvokeAsync<bool>("localStorage.hasOwnProperty", ct, key)
            .ConfigureAwait(false);
    }
    
    /// <summary>
    /// Gets the key name at the specified index in localStorage.
    /// </summary>
    /// <param name="index">The zero-based index</param>
    /// <param name="ct">Optional cancellation token</param>
    /// <returns>The key name at the specified index</returns>
    /// <exception cref="IndexOutOfRangeException">
    /// Thrown when the index is outside the valid range (0 to length-1)
    /// </exception>
    /// <remarks>
    /// Follows the same behavior as localStorage.key() in JavaScript.
    /// The iteration order is browser-defined but generally consistent.
    /// </remarks>
    public async ValueTask<string> KeyAsync(int index, CancellationToken ct = default)
    {
        return await jSRuntime
            .InvokeAsync<string>("localStorage.key", ct, index)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Gets all keys currently stored in localStorage.
    /// </summary>
    /// <param name="ct">Optional cancellation token</param>
    /// <returns>An enumerable collection of all keys</returns>
    /// <remarks>
    /// Uses JavaScript's Object.keys() on the localStorage object.
    /// The order of keys is browser-defined but generally consistent.
    /// </remarks>
    public async ValueTask<IEnumerable<string>> KeysAsync(CancellationToken ct = default)
    {
        return await jSRuntime
            .InvokeAsync<IEnumerable<string>>("eval", ct, "Object.keys(localStorage)")
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Gets the number of items stored in localStorage.
    /// </summary>
    /// <param name="ct">Optional cancellation token</param>
    /// <returns>The number of key-value pairs</returns>
    /// <remarks>
    /// Directly returns the localStorage.length property value.
    /// This count includes all keys for the current origin.
    /// </remarks>
    public async ValueTask<int> LengthAsync(CancellationToken ct = default)
    {
        return await jSRuntime
            .InvokeAsync<int>("eval", ct, "localStorage.length")
            .ConfigureAwait(false);
    }
    
    /// <summary>
    /// Clears all items from localStorage for the current origin.
    /// </summary>
    /// <param name="ct">Optional cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    /// <remarks>
    /// Warning: This removes ALL localStorage data for the current domain,
    /// including data not set by this application.
    /// Uses localStorage.clear() JavaScript API.
    /// </remarks>
    public async ValueTask ClearAsync(CancellationToken ct = default)
    {
        await jSRuntime
            .InvokeVoidAsync("localStorage.clear", ct)
            .ConfigureAwait(false);
    }
}