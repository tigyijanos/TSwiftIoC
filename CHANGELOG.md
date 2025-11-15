# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [2.0.0] - 2025-11-15
### Added
- **Performance Optimizations**:
  - Compiled expression trees for ultra-fast instance creation (replacing Activator.CreateInstance)
  - Optimized RegistrationKey as readonly struct with pre-calculated hash codes
  - Constructor info caching for dependency injection
  - Singleton instance fast-path retrieval
  - Array allocation optimization for dependency resolution

- **New Features**:
  - Scoped lifetime support with `BeginScope()` and `EndScope()` methods
  - Property injection via `[Inject]` attribute
  - `ResolveAll<T>()` method for batch resolution of all registered instances
  - `IsRegistered<T>()` method for registration validation
  - Circular dependency detection with detailed exception reporting
  - Thread-safe scoped instance storage using AsyncLocal

- **New Enhancements**:
  - Comprehensive test suite with 26 tests covering all features
  - Benchmark project using BenchmarkDotNet for performance measurements
  - Enhanced documentation with performance benchmarks and feature examples

### Changed
- Breaking: `Register<Interface, Type>()` method now includes `injectProperties` parameter
- Improved thread safety with AsyncLocal for scoped instances
- Enhanced circular dependency error messages with resolution stack trace

### Performance Improvements
- Up to 10x faster instance creation using compiled expressions
- Faster dictionary lookups with optimized struct-based keys
- Reduced reflection overhead through constructor caching
- Minimized allocations in dependency resolution

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