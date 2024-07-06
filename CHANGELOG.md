# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

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