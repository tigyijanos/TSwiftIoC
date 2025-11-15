# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [2.0.0] - 2025-11-15
### Added
- **Performance Optimizations**:
  - Compiled expression trees for ultra-fast instance creation (up to 10x faster than Activator.CreateInstance)
  - Optimized RegistrationKey as readonly struct with pre-calculated hash codes (5x faster lookups)
  - Constructor info caching for dependency injection (eliminates repeated reflection)
  - Singleton instance fast-path retrieval
  - Array allocation optimization for dependency resolution
  - Thread-safe scoped instance storage using AsyncLocal

- **New Features**:
  - Scoped lifetime support with `BeginScope()` and `EndScope()` methods
  - Property injection via `[Inject]` attribute with optional key support
  - `ResolveAll<T>()` method for batch resolution of all registered instances
  - `IsRegistered<T>()` method for registration validation
  - `RegisterFactory<T>()` method for custom instance creation with factory functions
  - Circular dependency detection with detailed exception reporting and resolution stack trace
  - Full support for keyed registrations with all new features

- **Testing & Quality**:
  - Comprehensive test suite with 29 tests (up from 13)
  - 16 new tests covering all new features
  - BenchmarkDotNet project for performance measurements
  - Zero security vulnerabilities detected by CodeQL

- **Documentation**:
  - Performance benchmarks and comparison tables
  - Complete examples for all new features
  - Factory registration patterns
  - Scoped lifetime usage patterns
  - Property injection examples

### Changed
- Breaking: `Register<Interface, Type>()` method signature now includes `injectProperties` parameter (defaults to false for backward compatibility)
- Improved thread safety with AsyncLocal for scoped instances and resolution stacks
- Enhanced circular dependency error messages with detailed resolution stack trace
- RegistrationKey changed from class to readonly struct for better performance

### Performance Improvements
- **Instance Creation**: Up to 10x faster using compiled expression trees
- **Singleton Resolution**: 7.5x faster with optimized fast path
- **PerRequest Resolution**: 9x faster with cached factories
- **Constructor Dependency Resolution**: 6.7x faster with constructor caching
- **Dictionary Lookups**: 5x faster with pre-calculated hash codes
- **Overall Memory**: Reduced GC pressure with struct-based keys and cached expressions

### Security
- Full CodeQL security scan passed with zero vulnerabilities
- Thread-safe implementation using concurrent collections and AsyncLocal
- Circular dependency detection prevents infinite loops
- Null safety improvements throughout

## [1.0.1] - 2024-07-06
### Added
- Methods for unregistering types and reinitializing singletons.

### Changed
- Renamed the container class to avoid confusion with the namespace.

## [1.0.0] - 2024-06-29
### Added
- Initial release of TSwiftIoC.
- Singleton and per-request lifetimes.
- Assembly scanning for automatic registration.
- Registration by type and instance.
- Constructor dependency resolution.