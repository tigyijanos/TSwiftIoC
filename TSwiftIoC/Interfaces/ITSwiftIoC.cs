using System.Reflection;
using TSwiftIoC.Enums;

namespace TSwiftIoC.Interfaces
{
    public interface ITSwiftIoC
    {
        void RegisterAssembly(Assembly assembly, Lifetime defaultLifetime = Lifetime.Singleton, bool resolveConstructorDependencies = false);

        void Register<Interface, Type>(string? key = null, Lifetime lifetime = Lifetime.Singleton, bool initializeOnRegister = false, bool resolveConstructorDependencies = false)
            where Type : class, Interface;

        void Register<Interface, Type>(Interface singletonInstance, string? key = null)
            where Type : class, Interface;

        Interface? Resolve<Interface>(string? key = null);
    }
}
