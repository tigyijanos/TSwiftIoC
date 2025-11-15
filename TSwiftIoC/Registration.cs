using System.Linq.Expressions;
using System.Reflection;
using TSwiftIoC.Enums;

namespace TSwiftIoC
{
    public class Registration
    {
        public Type ImplementationType { get; }
        public object? Instance { get; set; }
        public Lifetime Lifetime { get; }
        public bool ResolveConstructorDependencies { get; }
        public bool InjectProperties { get; }
        
        // Factory function support
        private readonly Func<object>? _customFactory;
        
        // Performance optimization: Cache compiled factory function
        private Func<object>? _cachedFactory;
        private ConstructorInfo? _cachedConstructor;
        private PropertyInfo[]? _injectableProperties;
        private readonly object _factoryLock = new object();

        public Registration(Type implementationType, Lifetime lifetime, bool resolveConstructorDependencies, bool injectProperties = false)
        {
            ImplementationType = implementationType;
            Lifetime = lifetime;
            Instance = null;
            ResolveConstructorDependencies = resolveConstructorDependencies;
            InjectProperties = injectProperties;
            
            // Pre-cache parameterless constructor info if not resolving dependencies
            if (!resolveConstructorDependencies)
            {
                _cachedConstructor = implementationType.GetConstructor(Type.EmptyTypes);
            }

            // Pre-cache injectable properties
            if (injectProperties)
            {
                _injectableProperties = implementationType
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.CanWrite && p.GetCustomAttribute<InjectAttribute>() != null)
                    .ToArray();
            }
        }

        public Registration(object instance)
        {
            ImplementationType = instance.GetType();
            Instance = instance;
            Lifetime = Lifetime.Singleton;
            ResolveConstructorDependencies = false;
            InjectProperties = false;
        }

        /// <summary>
        /// Registration with custom factory function
        /// </summary>
        public Registration(Type interfaceType, Func<object> factory, Lifetime lifetime)
        {
            ImplementationType = interfaceType;
            Lifetime = lifetime;
            Instance = null;
            ResolveConstructorDependencies = false;
            InjectProperties = false;
            _customFactory = factory;
        }

        /// <summary>
        /// Checks if this registration uses a custom factory
        /// </summary>
        public bool HasCustomFactory => _customFactory != null;

        /// <summary>
        /// Creates an instance using cached compiled expression for better performance
        /// </summary>
        public object CreateInstanceFast()
        {
            // Use custom factory if provided
            if (_customFactory != null)
            {
                return _customFactory();
            }

            if (!ResolveConstructorDependencies && _cachedFactory == null)
            {
                lock (_factoryLock)
                {
                    if (_cachedFactory == null)
                    {
                        // Use cached constructor or get it
                        var constructor = _cachedConstructor ?? ImplementationType.GetConstructor(Type.EmptyTypes);
                        if (constructor != null)
                        {
                            // Compile expression tree for fastest instantiation
                            var newExpr = Expression.New(constructor);
                            var lambda = Expression.Lambda<Func<object>>(Expression.Convert(newExpr, typeof(object)));
                            _cachedFactory = lambda.Compile();
                        }
                        else
                        {
                            // Fallback to Activator for types without parameterless constructor
                            _cachedFactory = () => Activator.CreateInstance(ImplementationType)!;
                        }
                    }
                }
            }

            return _cachedFactory != null ? _cachedFactory() : Activator.CreateInstance(ImplementationType)!;
        }

        /// <summary>
        /// Gets the cached constructor for dependency resolution
        /// </summary>
        public ConstructorInfo? GetCachedConstructor()
        {
            return _cachedConstructor;
        }

        /// <summary>
        /// Caches constructor for dependency resolution
        /// </summary>
        public void CacheConstructor(ConstructorInfo constructor)
        {
            _cachedConstructor = constructor;
        }

        /// <summary>
        /// Gets properties that should be injected
        /// </summary>
        public PropertyInfo[] GetInjectableProperties()
        {
            return _injectableProperties ?? Array.Empty<PropertyInfo>();
        }
    }
}
