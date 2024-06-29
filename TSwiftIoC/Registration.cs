using TSwiftIoC.Enums;

namespace TSwiftIoC
{
    public class Registration
    {
        public Type ImplementationType { get; }
        public object? Instance { get; set; }
        public Lifetime Lifetime { get; }
        public bool ResolveConstructorDependencies { get; }

        public Registration(Type implementationType, Lifetime lifetime, bool resolveConstructorDependencies)
        {
            ImplementationType = implementationType;
            Lifetime = lifetime;
            Instance = null;
            ResolveConstructorDependencies = resolveConstructorDependencies;
        }

        public Registration(object instance)
        {
            ImplementationType = instance.GetType();
            Instance = instance;
            Lifetime = Lifetime.Singleton;
            ResolveConstructorDependencies = false;
        }
    }
}
