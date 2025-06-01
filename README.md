# Toolkit.Blazor.Extensions.LocalStorage

Secure client-side localStorage implementation for Blazor applications with automatic encryption.

[![NuGet Version](https://img.shields.io/nuget/v/Toolkit.Blazor.Extensions.LocalStorage.svg)](https://www.nuget.org/packages/Toolkit.Blazor.Extensions.LocalStorage)
[![MIT License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

## Features

- **Secure Storage**: All values automatically encrypted using AES encryption
- **Type Safety**: Strongly-typed get/set operations for complex objects
- **Full API Coverage**: Complete implementation of the localStorage API
- **Async Operations**: All methods are async for Blazor compatibility
- **Cancellation Support**: Built-in support for CancellationToken

## Installation

```bash
dotnet add package Snail.Toolkit.Blazor.Extensions.LocalStorage
```

## Configuration

### Basic Setup

```csharp
builder.Services.AddLocalStorage();
```

### With Custom Encryption

```csharp
builder.Services.AddLocalStorage(options => 
{
    options.EncryptionKey = "your-256-bit-secret";
    options.IV = "your-16-byte-IV"; // Optional
});
```

### Using AppSettings Configuration

```json
{
  "Cryptography": {
    "EncryptionKey": "your-secure-key",
    "Iterations": 10000
  }
}
```

```csharp
builder.Services.AddLocalStorage(builder.Configuration);
```

## Usage

### Dependency Injection

```csharp
@inject ILocalStorage localStorage
```

### Basic Operations

**Store data:**
```csharp
await localStorage.SaveStringAsync("username", "john.doe");
await localStorage.SaveObjectAsync("user", new { Id = 1, Name = "John" });
```

**Retrieve data:**
```csharp
var username = await localStorage.GetStringAsync("username");
var user = await localStorage.GetObjectAsync<User>("user");
```

**Check existence:**
```csharp
var exists = await localStorage.ContainKeyAsync("username");
```

**Remove data:**
```csharp
await localStorage.RemoveAsync("username");
```

### Advanced Operations

**Get all keys:**
```csharp
var allKeys = await localStorage.KeysAsync();
```

**Clear storage:**
```csharp
await localStorage.ClearAsync();
```

## Encryption Details

All values are encrypted using AES-256-CBC before being stored in localStorage, providing protection against:
- XSS attacks that might access localStorage
- Browser extensions reading localStorage
- Basic snooping of localStorage data

**Note:** While values are encrypted, keys remain visible in localStorage.

## Browser Support

Works with all modern browsers that support:
- Web Storage API
- Web Cryptography API (for encryption)
- Blazor WebAssembly (client-side)

## License

Toolkit.Blazor.Extensions.LocalStorage is released under the [MIT License](LICENSE).