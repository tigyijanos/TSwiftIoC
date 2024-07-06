# TSwiftIoC ![NuGet](https://img.shields.io/nuget/v/TSwiftIoC)

TSwiftIoC is a simple and flexible Inversion of Control (IoC) container for .NET applications. It provides a way to register and resolve dependencies, supporting various lifetimes and constructor dependency resolution.

## Features

- Singleton and per-request lifetimes
- Assembly scanning for automatic registration
- Registration by type and instance
- Constructor dependency resolution
- Unregister registered types
- Reinitialize singleton instances

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
The container supports singleton and per-request lifetimes. By default, all registrations are singletons. You can specify the lifetime when registering a type:

```csharp
TSwiftContainer.Instance.Register<IService, Service>(lifetime: Lifetime.PerRequest);
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
