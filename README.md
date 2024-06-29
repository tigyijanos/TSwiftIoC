# TSwiftIoC

TSwiftIoC is a simple and flexible Inversion of Control (IoC) container for .NET applications. It provides a way to register and resolve dependencies, supporting various lifetimes and constructor dependency resolution.

## Features

- Singleton and per-request lifetimes
- Assembly scanning for automatic registration
- Registration by type and instance
- Constructor dependency resolution

## Installation

To use TSwiftIoC, include it in your project by copying the source code into your project.

## Usage

### Setting the IoC Container Type

You can set the IoC container type to a custom implementation by calling the `SetIoCType<T>()` method:

```csharp
TSwiftIoC.SetIoCType<MyCustomIoC>();
```

## Registering Dependencies
### Register by Type
You can register a type to be resolved by its interface:

```csharp
TSwiftIoC.Instance.Register<IService, Service>();
```

Register a Singleton Instance
You can register a singleton instance directly:

```csharp
var myService = new Service();
TSwiftIoC.Instance.Register<IService, Service>(myService);
```

Register All Types in an Assembly
You can register all types in an assembly that implement interfaces in that assembly:

```csharp
var assembly = Assembly.GetExecutingAssembly();
TSwiftIoC.Instance.RegisterAssembly(assembly);
```

Resolving Dependencies

You can resolve a dependency by its interface:
```csharp
var service = TSwiftIoC.Instance.Resolve<IService>();
```

Lifetime Management
The container supports singleton and per-request lifetimes. By default, all registrations are singletons. You can specify the lifetime when registering a type:

TSwiftIoC.Instance.Register<IService, Service>(lifetime: Lifetime.PerRequest);

Example
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
            TSwiftIoC.Instance.Register<IService, Service>();

            // Resolve dependencies
            var service = TSwiftIoC.Instance.Resolve<IService>();
            service?.DoSomething();
        }
    }
}
```

License
This project is licensed under the MIT License. See the LICENSE file for more details.

Contributing
Contributions are welcome! Please submit a pull request or open an issue to discuss changes.

Contact
For any questions or issues, please open an issue on the repository or contact the maintainers.