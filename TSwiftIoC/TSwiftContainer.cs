using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using TSwiftIoC.Enums;
using TSwiftIoC.Interfaces;

namespace TSwiftIoC
{
    public class TSwiftContainer : ITSwiftContainer
    {
        protected static Type? IoCType { get; set; } = typeof(TSwiftContainer);
        protected static ITSwiftContainer? _instance;
        protected readonly ConcurrentDictionary<RegistrationKey, Registration> _registrations = new();
        
        // Performance optimization: Cache resolved constructors for dependency injection
        private readonly ConcurrentDictionary<Type, ConstructorInfo> _constructorCache = new();
        
        // Scoped instances storage (thread-safe)
        private readonly AsyncLocal<Dictionary<RegistrationKey, object>> _scopedInstances = new();
        
        // Circular dependency detection
        private readonly AsyncLocal<Stack<Type>> _resolutionStack = new();

        public static ITSwiftContainer? Instance
        {
            get
            {
                if (IoCType == null)
                {
                    return null;
                }

                if (_instance == null)
                {
                    lock (typeof(TSwiftContainer))
                    {
                        if (_instance == null)
                        {
                            var instance = Activator.CreateInstance(IoCType);
                            _instance = instance != null ? (ITSwiftContainer)instance : null;
                        }
                    }
                }
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        public static void SetIoCType<T>() where T : ITSwiftContainer
        {
            IoCType = typeof(T);
            ClearRegistrations();
            _instance = null;
        }

        public virtual void RegisterAssembly(Assembly assembly, Lifetime defaultLifetime = Lifetime.Singleton, bool resolveConstructorDependencies = false)
        {
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                var interfaces = type.GetInterfaces();
                foreach (var @interface in interfaces)
                {
                    if (@interface.Assembly == assembly && !type.IsAbstract && !type.IsInterface)
                    {
                        Register(@interface, type, defaultLifetime, resolveConstructorDependencies: resolveConstructorDependencies);
                    }
                }
            }
        }

        public virtual void Register<Interface, Type>(string? key = null, Lifetime lifetime = Lifetime.Singleton, bool initializeOnRegister = false, bool resolveConstructorDependencies = false, bool injectProperties = false)
           where Type : class, Interface
        {
            var registrationKey = new RegistrationKey(typeof(Interface), key);
            if (_registrations.ContainsKey(registrationKey))
            {
                throw new InvalidOperationException($"Type {typeof(Type).Name} is already registered for interface {typeof(Interface).Name} with key {key ?? "default"}.");
            }

            _registrations[registrationKey] = new Registration(typeof(Type), lifetime, resolveConstructorDependencies, injectProperties);
            if (initializeOnRegister && lifetime == Lifetime.Singleton)
            {
                _registrations[registrationKey].Instance = CreateInstance(typeof(Interface), resolveConstructorDependencies, key);
            }
        }

        public virtual void Register<Interface, Type>(Interface singletonInstance, string? key = null)
            where Type : class, Interface
        {
            if (singletonInstance == null)
            {
                throw new ArgumentNullException(nameof(singletonInstance));
            }

            var registrationKey = new RegistrationKey(typeof(Interface), key);
            if (_registrations.ContainsKey(registrationKey))
            {
                throw new InvalidOperationException($"An instance is already registered for interface {typeof(Interface).Name} with key {key ?? "default"}.");
            }

            _registrations[registrationKey] = new Registration(singletonInstance);
        }

        public virtual Interface? Resolve<Interface>(string? key = null)
        {
            var res = Resolve(typeof(Interface), key);

            if (res != null)
            {
                return (Interface)res;
            }

            return default;
        }

        public virtual IEnumerable<Interface> ResolveAll<Interface>()
        {
            var interfaceType = typeof(Interface);
            var results = new List<Interface>();

            foreach (var kvp in _registrations)
            {
                if (kvp.Key.InterfaceType == interfaceType)
                {
                    var instance = Resolve(interfaceType, kvp.Key.Key);
                    if (instance != null)
                    {
                        results.Add((Interface)instance);
                    }
                }
            }

            return results;
        }

        public virtual bool IsRegistered<Interface>(string? key = null)
        {
            var registrationKey = new RegistrationKey(typeof(Interface), key);
            return _registrations.ContainsKey(registrationKey);
        }

        public virtual void BeginScope()
        {
            _scopedInstances.Value = new Dictionary<RegistrationKey, object>();
        }

        public virtual void EndScope()
        {
            _scopedInstances.Value?.Clear();
            _scopedInstances.Value = null!;
        }

        protected virtual void Register(Type @interface, Type implementation, Lifetime lifetime, bool resolveConstructorDependencies)
        {
            var registrationKey = new RegistrationKey(@interface, null);
            if (_registrations.ContainsKey(registrationKey))
            {
                throw new InvalidOperationException($"Type {implementation.Name} is already registered for interface {@interface.Name}.");
            }

            _registrations[registrationKey] = new Registration(implementation, lifetime, resolveConstructorDependencies, false);
        }

        protected virtual object? Resolve(Type type, string? key = null)
        {
            var registrationKey = new RegistrationKey(type, key);
            if (!_registrations.TryGetValue(registrationKey, out Registration? value))
            {
                throw new InvalidOperationException($"Type {type.Name} not registered with key {key ?? "default"}");
            }

            var registration = value;

            // Fast path for singleton instances
            if (registration.Instance != null)
            {
                return registration.Instance;
            }

            // Check for scoped instances
            if (registration.Lifetime == Lifetime.Scoped)
            {
                var scopedInstances = _scopedInstances.Value;
                if (scopedInstances != null)
                {
                    if (scopedInstances.TryGetValue(registrationKey, out var scopedInstance))
                    {
                        return scopedInstance;
                    }

                    var newInstance = CreateInstanceWithCircularDetection(type, registration.ResolveConstructorDependencies, key);
                    if (newInstance != null)
                    {
                        scopedInstances[registrationKey] = newInstance;
                    }
                    return newInstance;
                }
                // If no scope is active, treat as PerRequest
            }

            var instance = CreateInstanceWithCircularDetection(type, registration.ResolveConstructorDependencies, key);
            
            // Cache singleton instances
            if (registration.Lifetime == Lifetime.Singleton && instance != null)
            {
                registration.Instance = instance;
            }

            return instance;
        }

        private object? CreateInstanceWithCircularDetection(Type type, bool resolveConstructorDependencies, string? key)
        {
            // Initialize resolution stack if needed
            if (_resolutionStack.Value == null)
            {
                _resolutionStack.Value = new Stack<Type>();
            }

            var stack = _resolutionStack.Value;

            // Check for circular dependency
            if (stack.Contains(type))
            {
                throw new CircularDependencyException($"Circular dependency detected for type {type.Name}", stack);
            }

            try
            {
                stack.Push(type);
                return CreateInstance(type, resolveConstructorDependencies, key);
            }
            finally
            {
                stack.Pop();
                if (stack.Count == 0)
                {
                    _resolutionStack.Value = null!;
                }
            }
        }

        public virtual void Unregister<Interface>(string? key = null)
        {
            var registrationKey = new RegistrationKey(typeof(Interface), key);
            _registrations.TryRemove(registrationKey, out _);
        }

        public virtual void ReInitialize<Interface>(string? key = null)
        {
            var registrationKey = new RegistrationKey(typeof(Interface), key);
            if (!_registrations.TryGetValue(registrationKey, out Registration? registration))
            {
                throw new InvalidOperationException($"Type {typeof(Interface).Name} not registered with key {key ?? "default"}");
            }

            if (registration.Lifetime != Lifetime.Singleton)
            {
                throw new InvalidOperationException($"ReInitialize can only be called for singleton registrations.");
            }

            registration.Instance = CreateInstance(typeof(Interface), registration.ResolveConstructorDependencies, key);
        }

        protected static void ClearRegistrations()
        {
            if (_instance is TSwiftContainer container)
            {
                container._registrations.Clear();
                container._constructorCache.Clear();
            }
        }

        protected virtual object? CreateInstance(Type type, bool resolveConstructorDependencies, string? key = null)
        {
            var registrationKey = new RegistrationKey(type, key);
            if (!_registrations.TryGetValue(registrationKey, out Registration? registration))
            {
                throw new InvalidOperationException($"Type {type.Name} not registered with key {key ?? "default"}");
            }

            object? instance;
            
            if (resolveConstructorDependencies)
            {
                instance = CreateInstanceWithDependencies(registration);
            }
            else
            {
                // Use optimized factory method
                instance = registration.CreateInstanceFast();
            }

            // Inject properties if configured
            if (instance != null && registration.InjectProperties)
            {
                InjectProperties(instance, registration);
            }

            return instance;
        }

        /// <summary>
        /// Injects dependencies into properties marked with [Inject] attribute
        /// </summary>
        private void InjectProperties(object instance, Registration registration)
        {
            var properties = registration.GetInjectableProperties();
            foreach (var property in properties)
            {
                var attribute = property.GetCustomAttribute<InjectAttribute>();
                var key = attribute?.Key;
                var value = Resolve(property.PropertyType, key);
                if (value != null)
                {
                    property.SetValue(instance, value);
                }
            }
        }

        /// <summary>
        /// Optimized method for creating instances with constructor dependency resolution
        /// </summary>
        private object CreateInstanceWithDependencies(Registration registration)
        {
            // Try to get cached constructor first
            var cachedConstructor = registration.GetCachedConstructor();
            if (cachedConstructor != null)
            {
                var parameters = cachedConstructor.GetParameters();
                var parameterInstances = new object[parameters.Length];
                for (int i = 0; i < parameters.Length; i++)
                {
                    parameterInstances[i] = Resolve(parameters[i].ParameterType)!;
                }
                return cachedConstructor.Invoke(parameterInstances);
            }

            // Find and cache constructor
            var constructors = registration.ImplementationType
                .GetConstructors()
                .OrderByDescending(c => c.GetParameters().Length)
                .ToArray();

            foreach (var constructor in constructors)
            {
                try
                {
                    var parameters = constructor.GetParameters();
                    var parameterInstances = new object[parameters.Length];
                    
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        parameterInstances[i] = Resolve(parameters[i].ParameterType)!;
                    }
                    
                    // Cache the successful constructor
                    registration.CacheConstructor(constructor);
                    return constructor.Invoke(parameterInstances);
                }
                catch (CircularDependencyException)
                {
                    // Re-throw circular dependency exceptions
                    throw;
                }
                catch
                {
                    continue;
                }
            }

            throw new InvalidOperationException($"No suitable constructor found for type {registration.ImplementationType.Name}");
        }
    }
}