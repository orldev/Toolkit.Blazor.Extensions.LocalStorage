using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Toolkit.Blazor.Extensions.Cryptography.Extensions;
using Toolkit.Cryptography.Entities;


namespace Toolkit.Blazor.Extensions.LocalStorage;

/// <summary>
/// Provides extension methods for <see cref="IServiceCollection"/> to configure browser localStorage services.
/// </summary>
/// <remarks>
/// These extensions register the required services to work with the browser's localStorage API
/// in a Blazor WebAssembly or other JavaScript-interop scenarios.
/// 
/// The implementation includes:
/// - Encryption of all stored values using symmetric cryptography
/// - Async API surface matching browser localStorage capabilities
/// - JSON serialization for complex objects
/// 
/// Note: These services are primarily designed for client-side Blazor applications.
/// </remarks>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds localStorage services to the service collection with optional cryptographic configuration.
    /// </summary>
    /// <param name="services">The service collection to add to</param>
    /// <param name="options">Optional action to configure symmetric cryptography options</param>
    /// <returns>The service collection for chaining</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> is null</exception>
    /// <remarks>
    /// This registration:
    /// 1. Configures symmetric cryptography using <see cref="SymCryptoOpts"/>
    /// 2. Registers <see cref="ILocalStorage"/> as scoped service with <see cref="LocalStorage"/> implementation
    /// 
    /// Example:
    /// <code>
    /// builder.Services.AddLocalStorage(opts => {
    ///     opts.EncryptionKey = "my-secret-key";
    /// });
    /// </code>
    /// </remarks>
    public static IServiceCollection AddLocalStorage(
        this IServiceCollection services, 
        Action<SymCryptoOpts>? options = null)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddJsSymmetricCipher(options);
        services.TryAddScoped<ILocalStorage, LocalStorage>();
        return services;
    }
    
    /// <summary>
    /// Adds localStorage services to the service collection with cryptographic configuration from IConfiguration.
    /// </summary>
    /// <param name="services">The service collection to add to</param>
    /// <param name="configuration">Configuration containing symmetric cryptography settings</param>
    /// <returns>The service collection for chaining</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> or <paramref name="configuration"/> is null</exception>
    /// <remarks>
    /// This registration:
    /// 1. Configures symmetric cryptography using settings from <see cref="IConfiguration"/>
    /// 2. Registers <see cref="ILocalStorage"/> as scoped service with <see cref="LocalStorage"/> implementation
    /// 
    /// Expects configuration structure:
    /// <code>
    /// {
    ///   "SymmetricCryptography": {
    ///     "EncryptionKey": "your-encryption-key",
    ///     "Iterations": 10000
    ///   }
    /// }
    /// </code>
    /// </remarks>
    public static IServiceCollection AddLocalStorage(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddJsSymmetricCipher(configuration);
        services.TryAddScoped<ILocalStorage, LocalStorage>();
        return services;
    }
}