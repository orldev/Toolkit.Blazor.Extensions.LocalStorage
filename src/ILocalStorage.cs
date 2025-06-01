using System.Text.Json;

namespace Toolkit.Blazor.Extensions.LocalStorage;

/// <summary>
/// Represents an asynchronous interface for interacting with the browser's localStorage.
/// localStorage provides a way to store key-value pairs in a web browser with no expiration time.
/// </summary>
/// <remarks>
/// This interface abstracts the browser's localStorage API to provide async operations
/// that can be used in Blazor WebAssembly or other .NET web applications.
/// Data stored in localStorage remains even when the browser is closed and reopened.
/// Storage limit is typically around 5MB per origin (domain).
/// </remarks>
public interface ILocalStorage
{
    /// <summary>
    /// Removes the item with the specified key from localStorage.
    /// </summary>
    /// <param name="key">The key of the item to remove.</param>
    /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous remove operation.</returns>
    public Task RemoveAsync(string key, CancellationToken ct = default);

    /// <summary>
    /// Stores a string value with the specified key in localStorage.
    /// </summary>
    /// <param name="key">The key to associate with the value.</param>
    /// <param name="value">The string value to store.</param>
    /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous save operation.</returns>
    public Task SaveStringAsync(string key, string value, CancellationToken ct = default);
    
    /// <summary>
    /// Retrieves the string value associated with the specified key from localStorage.
    /// </summary>
    /// <param name="key">The key of the value to retrieve.</param>
    /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>
    /// A ValueTask that represents the asynchronous operation, containing the string value
    /// or null if the key does not exist.
    /// </returns>
    public ValueTask<string?> GetStringAsync(string key, CancellationToken ct = default);
    
    /// <summary>
    /// Serializes and stores an object with the specified key in localStorage.
    /// </summary>
    /// <typeparam name="T">The type of the object to store.</typeparam>
    /// <param name="key">The key to associate with the object.</param>
    /// <param name="value">The object to store.</param>
    /// <param name="options">Optional JSON serializer options.</param>
    /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous save operation.</returns>
    public Task SaveObjectAsync<T>(string key, T value, JsonSerializerOptions? options = null, CancellationToken ct = default);
    
    /// <summary>
    /// Retrieves and deserializes an object associated with the specified key from localStorage.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the object to.</typeparam>
    /// <param name="key">The key of the object to retrieve.</param>
    /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>
    /// A ValueTask that represents the asynchronous operation, containing the deserialized object
    /// or null if the key does not exist or deserialization fails.
    /// </returns>
    public ValueTask<T?> GetObjectAsync<T>(string key, CancellationToken ct = default);
    
    /// <summary>
    /// Checks whether localStorage contains an item with the specified key.
    /// </summary>
    /// <param name="key">The key to check for existence.</param>
    /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>
    /// A ValueTask that represents the asynchronous operation, containing true if the key exists,
    /// false otherwise.
    /// </returns>
    ValueTask<bool> ContainKeyAsync(string key, CancellationToken ct = default);

    /// <summary>
    /// Gets the name of the key at the specified index in localStorage.
    /// </summary>
    /// <param name="index">The zero-based index of the key to retrieve.</param>
    /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>
    /// A ValueTask that represents the asynchronous operation, containing the key name
    /// at the specified index.
    /// </returns>
    /// <exception cref="System.IndexOutOfRangeException">
    /// Thrown when the index is out of range (negative or >= length).
    /// </exception>
    ValueTask<string> KeyAsync(int index, CancellationToken ct = default);

    /// <summary>
    /// Gets all keys stored in localStorage.
    /// </summary>
    /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>
    /// A ValueTask that represents the asynchronous operation, containing an enumerable
    /// of all keys in localStorage.
    /// </returns>
    ValueTask<IEnumerable<string>> KeysAsync(CancellationToken ct = default);
    
    /// <summary>
    /// Gets the number of items stored in localStorage.
    /// </summary>
    /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>
    /// A ValueTask that represents the asynchronous operation, containing the number
    /// of items in localStorage.
    /// </returns>
    ValueTask<int> LengthAsync(CancellationToken ct = default);
    
    /// <summary>
    /// Removes all items from localStorage.
    /// </summary>
    /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A ValueTask that represents the asynchronous clear operation.</returns>
    /// <remarks>
    /// Note: This clears ALL data for the current origin (domain) in localStorage,
    /// not just data set by your application.
    /// </remarks>
    ValueTask ClearAsync(CancellationToken ct = default);
}