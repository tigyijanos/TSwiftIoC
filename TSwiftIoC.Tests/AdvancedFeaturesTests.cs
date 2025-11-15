using TSwiftIoC.Enums;
using TSwiftIoC.Tests.Models.Interfaces;

namespace TSwiftIoC.Tests
{
    // Test models for circular dependency and property injection
    public interface ICircularA { }
    public interface ICircularB { }
    
    public class CircularA : ICircularA
    {
        public CircularA(ICircularB b) { }
    }
    
    public class CircularB : ICircularB
    {
        public CircularB(ICircularA a) { }
    }

    public interface IPropertyService { }
    public class PropertyService : IPropertyService { }

    public interface IServiceWithPropertyInjection
    {
        IPropertyService? InjectedProperty { get; set; }
        IPropertyService? NotInjectedProperty { get; set; }
    }

    public class ServiceWithPropertyInjection : IServiceWithPropertyInjection
    {
        [Inject]
        public IPropertyService? InjectedProperty { get; set; }

        public IPropertyService? NotInjectedProperty { get; set; }
    }

    [Collection("Sequential")]
    public class AdvancedFeaturesTests
    {
        public AdvancedFeaturesTests()
        {
            TSwiftContainer.Instance = null;
        }

        [Fact]
        public void Circular_Dependency_Throws_Exception()
        {
            // Arrange
            var container = TSwiftContainer.Instance;
            container!.Register<ICircularA, CircularA>(resolveConstructorDependencies: true);
            container.Register<ICircularB, CircularB>(resolveConstructorDependencies: true);

            // Act & Assert
            var exception = Assert.Throws<CircularDependencyException>(() => container.Resolve<ICircularA>());
            Assert.Contains("CircularA", exception.Message);
        }

        [Fact]
        public void Property_Injection_Works()
        {
            // Arrange
            var container = TSwiftContainer.Instance;
            container!.Register<IPropertyService, PropertyService>();
            container.Register<IServiceWithPropertyInjection, ServiceWithPropertyInjection>(injectProperties: true);

            // Act
            var service = container.Resolve<IServiceWithPropertyInjection>();

            // Assert
            Assert.NotNull(service);
            Assert.NotNull(service!.InjectedProperty);
            Assert.Null(service.NotInjectedProperty);
        }

        [Fact]
        public void Property_Injection_Without_Flag_Does_Not_Inject()
        {
            // Arrange
            var container = TSwiftContainer.Instance;
            container!.Register<IPropertyService, PropertyService>();
            container.Register<IServiceWithPropertyInjection, ServiceWithPropertyInjection>(injectProperties: false);

            // Act
            var service = container.Resolve<IServiceWithPropertyInjection>();

            // Assert
            Assert.NotNull(service);
            Assert.Null(service!.InjectedProperty);
            Assert.Null(service.NotInjectedProperty);
        }

        [Fact]
        public void Property_Injection_With_Constructor_Dependencies()
        {
            // Arrange
            var container = TSwiftContainer.Instance;
            container!.Register<IPropertyService, PropertyService>();
            container.Register<IServiceWithPropertyInjection, ServiceWithPropertyInjection>(
                resolveConstructorDependencies: true, 
                injectProperties: true);

            // Act
            var service = container.Resolve<IServiceWithPropertyInjection>();

            // Assert
            Assert.NotNull(service);
            Assert.NotNull(service!.InjectedProperty);
        }
    }
}
