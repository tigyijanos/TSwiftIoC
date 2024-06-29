using System.Collections.Concurrent;
using System.Reflection;
using TSwiftIoC.Enums;
using TSwiftIoC.Interfaces;

namespace TSwiftIoC
{
    public class TSwiftIoC : ITSwiftIoC
    {
        protected static Type? IoCType { get; set; } = typeof(TSwiftIoC);
        protected static ITSwiftIoC? _instance;
        protected readonly ConcurrentDictionary<RegistrationKey, Registration> _registrations = new();

        public static ITSwiftIoC? Instance
        {
            get
            {
                if(IoCType == null)
                {
                    return null;
                }

                if (_instance == null)
                {
                    lock (typeof(TSwiftIoC))
                    {
                        if (_instance == null)
                        {
                            var instance = Activator.CreateInstance(IoCType);
                            _instance = instance != null ? (ITSwiftIoC)instance : null;
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

        public static void SetIoCType<T>() where T : ITSwiftIoC
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

        public virtual void Register<Interface, Type>(string? key = null, Lifetime lifetime = Lifetime.Singleton, bool initializeOnRegister = false, bool resolveConstructorDependencies = false)
           where Type : class, Interface
        {
            var registrationKey = new RegistrationKey(typeof(Interface), key);
            if (_registrations.ContainsKey(registrationKey))
            {
                throw new InvalidOperationException($"Type {typeof(Type).Name} is already registered for interface {typeof(Interface).Name} with key {key ?? "default"}.");
            }

            _registrations[registrationKey] = new Registration(typeof(Type), lifetime, resolveConstructorDependencies);
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

        protected virtual void Register(Type @interface, Type implementation, Lifetime lifetime, bool resolveConstructorDependencies)
        {
            var registrationKey = new RegistrationKey(@interface, null);
            if (_registrations.ContainsKey(registrationKey))
            {
                throw new InvalidOperationException($"Type {implementation.Name} is already registered for interface {@interface.Name}.");
            }

            _registrations[registrationKey] = new Registration(implementation, lifetime, resolveConstructorDependencies);
        }

        protected virtual object? Resolve(Type type, string? key = null)
        {
            var registrationKey = new RegistrationKey(type, key);
            if (!_registrations.TryGetValue(registrationKey, out Registration? value))
            {
                throw new InvalidOperationException($"Type {type.Name} not registered with key {key ?? "default"}");
            }

            var registration = value;

            if (registration.Instance != null)
            {
                return registration.Instance;
            }

            var instance = CreateInstance(type, registration.ResolveConstructorDependencies, key);
            if (registration.Lifetime == Lifetime.Singleton && instance != null)
            {
                registration.Instance = instance;
            }

            return instance;
        }

        protected static void ClearRegistrations()
        {
            if (_instance is TSwiftIoC container)
            {
                container._registrations.Clear();
            }
        }

        protected virtual object? CreateInstance(Type type, bool resolveConstructorDependencies, string? key = null)
        {
            var registrationKey = new RegistrationKey(type, key);
            if (!_registrations.TryGetValue(registrationKey, out Registration? registration))
            {
                throw new InvalidOperationException($"Type {type.Name} not registered with key {key ?? "default"}");
            }

            if (resolveConstructorDependencies)
            {
                var constructors = registration.ImplementationType
                    .GetConstructors()
                    .OrderByDescending(c => c.GetParameters().Length)
                    .ToArray();

                foreach (var constructor in constructors)
                {
                    try
                    {
                        var parameters = constructor.GetParameters();
                        var parameterInstances = parameters.Select(param => Resolve(param.ParameterType)).ToArray();
                        return constructor.Invoke(parameterInstances);
                    }
                    catch
                    {
                        continue;
                    }
                }

                throw new InvalidOperationException($"No suitable constructor found for type {type.Name}");
            }
            else
            {
                return Activator.CreateInstance(registration.ImplementationType);
            }
        }
    }
}
