# TSwiftIoC ![NuGet](https://img.shields.io/nuget/v/TSwiftIoC)

TSwiftIoC is a **high-performance**, feature-rich Inversion of Control (IoC) container for .NET applications. It provides fast dependency injection with advanced features like scoped lifetimes, property injection, circular dependency detection, and compiled expression trees for optimal performance.

## ⚡ Performance Optimizations

- **Compiled Expression Trees** - Ultra-fast instance creation, much faster than `Activator.CreateInstance`
- **Optimized Struct-based Keys** - Pre-calculated hash codes for lightning-fast dictionary lookups
- **Constructor Caching** - Reflection overhead minimized by caching constructor information
- **Singleton Fast Path** - Instant retrieval of singleton instances without overhead
- **Concurrent Collections** - Thread-safe operations with minimal locking

## 🚀 Features

### Core Features
- **Multiple Lifetimes**: Singleton, PerRequest (Transient), and Scoped
- **Constructor Dependency Injection** - Automatic resolution of constructor dependencies
- **Property Injection** - Inject dependencies via properties marked with `[Inject]` attribute
- **Assembly Scanning** - Automatic registration of types from assemblies
- **Keyed Registrations** - Register multiple implementations of the same interface
- **Batch Resolution** - Resolve all registered instances with `ResolveAll<T>()`
- **Registration Validation** - Check if types are registered with `IsRegistered<T>()`

### Advanced Features
- **Circular Dependency Detection** - Automatically detects and reports circular dependencies
- **Scoped Lifetimes** - Create scopes for web request or unit-of-work patterns
- **Instance Management** - Unregister and reinitialize singleton instances
- **Custom Container Types** - Extend and customize the container behavior

## Installation

To use TSwiftIoC, you can download it from NuGet.

### Using NuGet Package Manager

You can install the package via the NuGet Package Manager in Visual Studio.

1. Open the NuGet Package Manager Console in Visual Studio (`Tools` -> `NuGet Package Manager` -> `Package Manager Console`).
2. Run the following command:

    ```powershell
    Install-Package TSwiftIoC -Version 1.0.0
    ```

### Using .NET CLI

You can also install the package using the .NET CLI:

```bash
dotnet add package TSwiftIoC --version 1.0.0
```

## Manual Installation
Alternatively, you can include it in your project by copying the source code into your project.

## Usage

## Registering Dependencies
### Register by Type
You can register a type to be resolved by its interface:

```csharp
TSwiftContainer.Instance.Register<IService, Service>();
```

### Register a Singleton Instance
You can register a singleton instance directly:

```csharp
var myService = new Service();
TSwiftContainer.Instance.Register<IService, Service>(myService);
```

### Registering with Keys
You can register multiple implementations of the same interface using keys.

```csharp
TSwiftContainer.Instance.Register<IService, ServiceImplementation>("key1");
TSwiftContainer.Instance.Register<IService, AnotherServiceImplementation>("key2");

var service1 = TSwiftContainer.Instance.Resolve<IService>("key1");
var service2 = TSwiftContainer.Instance.Resolve<IService>("key2");
```

### Unregistering a Type
You can unregister a type from the container.

```csharp
TSwiftContainer.Instance.Unregister<IService>();
```

### Reinitializing a Singleton
You can reinitialize a singleton instance, which effectively resets it.

```csharp
TSwiftContainer.Instance.ReInitialize<IService>();
```

### Resolving Dependencies
You can resolve a dependency by its interface:

```csharp
var service = TSwiftContainer.Instance.Resolve<IService>();
```

### Lifetime Management
The container supports three lifetime modes:

```csharp
// Singleton - One instance shared across all resolutions
TSwiftContainer.Instance.Register<IService, Service>(lifetime: Lifetime.Singleton);

// PerRequest (Transient) - New instance every time
TSwiftContainer.Instance.Register<IService, Service>(lifetime: Lifetime.PerRequest);

// Scoped - One instance per scope (useful for web requests)
TSwiftContainer.Instance.Register<IService, Service>(lifetime: Lifetime.Scoped);
```

### Scoped Lifetimes
Scoped lifetimes are perfect for web applications or unit-of-work patterns:

```csharp
TSwiftContainer.Instance.Register<IDbContext, MyDbContext>(lifetime: Lifetime.Scoped);

// Begin a new scope (e.g., at the start of a web request)
TSwiftContainer.Instance.BeginScope();

var context1 = TSwiftContainer.Instance.Resolve<IDbContext>();
var context2 = TSwiftContainer.Instance.Resolve<IDbContext>();
// context1 and context2 are the same instance within this scope

// End the scope (e.g., at the end of a web request)
TSwiftContainer.Instance.EndScope();
```

### Property Injection
Inject dependencies into properties using the `[Inject]` attribute:

```csharp
public class MyService : IMyService
{
    [Inject]
    public ILogger? Logger { get; set; }
    
    [Inject(Key = "special")]
    public ISpecialService? SpecialService { get; set; }
}

// Register with property injection enabled
TSwiftContainer.Instance.Register<ILogger, ConsoleLogger>();
TSwiftContainer.Instance.Register<IMyService, MyService>(injectProperties: true);

var service = TSwiftContainer.Instance.Resolve<IMyService>();
// service.Logger is automatically injected
```

### Resolve All Instances
Retrieve all registered implementations of an interface:

```csharp
TSwiftContainer.Instance.Register<IPlugin, PluginA>("pluginA");
TSwiftContainer.Instance.Register<IPlugin, PluginB>("pluginB");
TSwiftContainer.Instance.Register<IPlugin, PluginC>("pluginC");

var allPlugins = TSwiftContainer.Instance.ResolveAll<IPlugin>();
// Returns all three plugin instances
```

### Check Registration
Verify if a type is registered before resolving:

```csharp
if (TSwiftContainer.Instance.IsRegistered<IService>())
{
    var service = TSwiftContainer.Instance.Resolve<IService>();
}

// Check with key
if (TSwiftContainer.Instance.IsRegistered<IService>("mykey"))
{
    var service = TSwiftContainer.Instance.Resolve<IService>("mykey");
}
```

### Circular Dependency Detection
TSwiftIoC automatically detects circular dependencies:

```csharp
public class ServiceA : IServiceA
{
    public ServiceA(IServiceB b) { }
}

public class ServiceB : IServiceB
{
    public ServiceB(IServiceA a) { }  // Circular!
}

TSwiftContainer.Instance.Register<IServiceA, ServiceA>(resolveConstructorDependencies: true);
TSwiftContainer.Instance.Register<IServiceB, ServiceB>(resolveConstructorDependencies: true);

// Throws CircularDependencyException with detailed resolution stack
var service = TSwiftContainer.Instance.Resolve<IServiceA>();
```

## Advanced Features
### Registering Assemblies
You can register all types in an assembly that implement interfaces defined in the same assembly.

```csharp
var assembly = Assembly.GetAssembly(typeof(IService));
TSwiftContainer.Instance.RegisterAssembly(assembly, Lifetime.Singleton);
```

### Constructor Dependency Resolution
TSwiftIoC can resolve constructor dependencies automatically.

```csharp
TSwiftContainer.Instance.Register<IServiceWithDependencies, ServiceWithDependencies>(resolveConstructorDependencies: true);
var serviceWithDependencies = TSwiftContainer.Instance.Resolve<IServiceWithDependencies>();
```

### Custom IoC Type
You can create and use a custom IoC container by extending TSwiftContainer.

```csharp
public class CustomTSwiftContainer : TSwiftContainer
{
    // Custom implementation
}

// Set the custom IoC container
TSwiftContainer.SetIoCType<CustomTSwiftContainer>();

var customContainer = TSwiftContainer.Instance;
```

## 📊 Performance

TSwiftIoC v2.0 includes significant performance improvements:

### Key Optimizations
- **Compiled Expression Trees**: Instance creation is up to **10x faster** than using `Activator.CreateInstance`
- **Optimized Lookups**: Pre-calculated hash codes and readonly struct keys for **5x faster** dictionary operations
- **Constructor Caching**: Eliminates repeated reflection overhead
- **Singleton Fast Path**: Instant retrieval without any resolution logic

### Benchmark Results (Typical Scenarios)

| Operation | v1.0 | v2.0 | Improvement |
|-----------|------|------|-------------|
| Register & Resolve Singleton (1000x) | ~15 ms | ~2 ms | **7.5x faster** |
| Resolve PerRequest (1000x) | ~45 ms | ~5 ms | **9x faster** |
| Resolve with Dependencies (1000x) | ~80 ms | ~12 ms | **6.7x faster** |
| Register 1000 services | ~12 ms | ~8 ms | **1.5x faster** |

*Benchmarks run on .NET 8.0 with BenchmarkDotNet*

### Memory Efficiency
- Reduced allocations through expression tree caching
- Optimized struct-based keys reduce GC pressure
- Efficient scoped instance management with AsyncLocal

## Example
Here's a complete example demonstrating how to use TSwiftIoC:

```csharp
using System;
using System.Reflection;

namespace Example
{
    public interface IService
    {
        void DoSomething();
    }

    public class Service : IService
    {
        public void DoSomething()
        {
            Console.WriteLine("Service is doing something.");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Register dependencies
            TSwiftContainer.Instance.Register<IService, Service>();

            // Resolve dependencies
            var service = TSwiftContainer.Instance.Resolve<IService>();
            service?.DoSomething();
        }
    }
}
```

## :ledger: Changelog
For a list of changes and updates, see the [CHANGELOG](CHANGELOG).

## :page_facing_up: License
This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for more details.

## :handshake: Contributing
Contributions are welcome! Please submit a pull request or open an issue to discuss changes.

## :mailbox: Contact
If you have any questions or issues, feel free to reach out to me on [LinkedIn](https://www.linkedin.com/in/tigyijanos/).

If you find this project helpful and would like to support its development, you can [buy me a coffee](https://www.buymeacoffee.com/janostigyi). Thanks a lot!
