﻿using System.Reflection;
using TSwiftIoC.Enums;

namespace TSwiftIoC.Interfaces
{
    /// <summary>
    /// Interface for ITSwiftContainer container providing methods for registration, resolution, and management of dependencies.
    /// </summary>
    public interface ITSwiftContainer
    {
        /// <summary>
        /// Registers all types in the specified assembly that implement interfaces defined in the same assembly.
        /// </summary>
        /// <param name="assembly">The assembly containing the types to register.</param>
        /// <param name="defaultLifetime">The default lifetime of the registered types.</param>
        /// <param name="resolveConstructorDependencies">Specifies whether to resolve constructor dependencies when creating instances.</param>
        void RegisterAssembly(Assembly assembly, Lifetime defaultLifetime = Lifetime.Singleton, bool resolveConstructorDependencies = false);

        /// <summary>
        /// Registers a type with the specified interface and optional key.
        /// </summary>
        /// <typeparam name="Interface">The interface type.</typeparam>
        /// <typeparam name="Type">The implementation type.</typeparam>
        /// <param name="key">An optional key to distinguish multiple registrations of the same interface.</param>
        /// <param name="lifetime">The lifetime of the registered type.</param>
        /// <param name="initializeOnRegister">Specifies whether to initialize the instance upon registration.</param>
        /// <param name="resolveConstructorDependencies">Specifies whether to resolve constructor dependencies when creating instances.</param>
        void Register<Interface, Type>(string? key = null, Lifetime lifetime = Lifetime.Singleton, bool initializeOnRegister = false, bool resolveConstructorDependencies = false)
            where Type : class, Interface;

        /// <summary>
        /// Registers a singleton instance with the specified interface and optional key.
        /// </summary>
        /// <typeparam name="Interface">The interface type.</typeparam>
        /// <typeparam name="Type">The implementation type.</typeparam>
        /// <param name="singletonInstance">The singleton instance to register.</param>
        /// <param name="key">An optional key to distinguish multiple registrations of the same interface.</param>
        void Register<Interface, Type>(Interface singletonInstance, string? key = null)
            where Type : class, Interface;

        /// <summary>
        /// Unregisters a type with the specified interface and optional key.
        /// </summary>
        /// <typeparam name="Interface">The interface type.</typeparam>
        /// <param name="key">An optional key to distinguish multiple registrations of the same interface.</param>
        void Unregister<Interface>(string? key = null);

        /// <summary>
        /// Reinitializes a singleton instance for the specified interface and optional key.
        /// </summary>
        /// <typeparam name="Interface">The interface type.</typeparam>
        /// <param name="key">An optional key to distinguish multiple registrations of the same interface.</param>
        void ReInitialize<Interface>(string? key = null);

        /// <summary>
        /// Resolves an instance of the specified interface and optional key.
        /// </summary>
        /// <typeparam name="Interface">The interface type.</typeparam>
        /// <param name="key">An optional key to distinguish multiple registrations of the same interface.</param>
        /// <returns>The resolved instance of the specified interface.</returns>
        Interface? Resolve<Interface>(string? key = null);
    }
}
