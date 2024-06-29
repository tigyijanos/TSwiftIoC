using System.Reflection;
using TSwiftIoC.Enums;
using TSwiftIoC.Tests.Models;
using TSwiftIoC.Tests.Models.Assembly;
using TSwiftIoC.Tests.Models.Interfaces;
using TSwiftIoC.Tests.Models.Interfaces.Assembly;

namespace TSwiftIoC.Tests
{
    public class TSwiftIoCTests
    {
        public TSwiftIoCTests()
        {
            // Ensure IoC container is reset before each test
            TSwiftIoC.Instance = null;
        }

        [Fact]
        public void Register_And_Resolve_Singleton_Instance_Success()
        {
            // Arrange
            var container = TSwiftIoC.Instance;
            container!.Register<IService, ServiceImplementation>(lifetime: Lifetime.Singleton, initializeOnRegister: true);

            // Act
            var service1 = container.Resolve<IService>();
            var service2 = container.Resolve<IService>();

            // Assert
            Assert.NotNull(service1);
            Assert.Equal(service1, service2); // Singleton should return the same instance
            Assert.Equal(service1.Guid, service2!.Guid);
        }

        [Fact]
        public void Register_And_Resolve_Transient_Instance_Success()
        {
            // Arrange
            var container = TSwiftIoC.Instance;
            container!.Register<IService, ServiceImplementation>(lifetime: Lifetime.PerRequest);

            // Act
            var service1 = container.Resolve<IService>();
            var service2 = container.Resolve<IService>();

            // Assert
            Assert.NotNull(service1);
            Assert.NotNull(service2);
            Assert.NotEqual(service1, service2); // PerRequest should return different instances
            Assert.NotEqual(service1.Guid, service2.Guid);
        }

        [Fact]
        public void Register_With_Key_And_Resolve_Success()
        {
            // Arrange
            var container = TSwiftIoC.Instance;
            container!.Register<IService, ServiceImplementation>("key1");
            container.Register<IService, AnotherServiceImplementation>("key2");

            // Act
            var service1 = container.Resolve<IService>("key1");
            var service2 = container.Resolve<IService>("key2");

            // Assert
            Assert.IsType<ServiceImplementation>(service1);
            Assert.IsType<AnotherServiceImplementation>(service2);
        }

        [Fact]
        public void Register_Duplicate_Without_Key_Throws_Exception()
        {
            // Arrange
            var container = TSwiftIoC.Instance;
            container!.Register<IService, ServiceImplementation>();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => container.Register<IService, AnotherServiceImplementation>());
        }

        [Fact]
        public void Register_Duplicate_With_Same_Key_Throws_Exception()
        {
            // Arrange
            var container = TSwiftIoC.Instance;
            container!.Register<IService, ServiceImplementation>("key");

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => container.Register<IService, AnotherServiceImplementation>("key"));
        }

        [Fact]
        public void Resolve_Not_Registered_Throws_Exception()
        {
            // Arrange
            var container = TSwiftIoC.Instance;

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => container!.Resolve<IService>());
        }

        [Fact]
        public void Register_Thausand_Of_Services()
        {
            // Arrange
            var container = TSwiftIoC.Instance;
            var x = 0;

            // Act & Assert
            while (x < 1000)
            {
                x++;

                container!.Register<IService, ServiceImplementation>($"test_key_{x}");
            }
        }

        [Fact]
        public void Custom_IoC_Type()
        {
            // Arrange
            TSwiftIoC.SetIoCType<CustomTSwiftIoC>();

            // Act
            var i = TSwiftIoC.Instance;

            // Assert
            Assert.IsType<CustomTSwiftIoC>(i);
        }

        [Fact]
        public void Register_Assembly_Success()
        {
            // Arrange
            var container = TSwiftIoC.Instance;
            var testAssembly = Assembly.GetAssembly(typeof(IService1));

            // Act
            container?.RegisterAssembly(testAssembly!, Lifetime.Singleton);

            // Assert
            var service1 = container!.Resolve<IService1>();
            var service2 = container!.Resolve<IService2>();

            Assert.IsType<ServiceImplementation1>(service1);
            Assert.IsType<ServiceImplementation2>(service2);
        }        
        
        [Fact]
        public void Register_Resolve_Constructor_Params_Success()
        {
            // Arrange
            var container = TSwiftIoC.Instance;

            // Act
            container!.Register<IServiceInConstructorParam, ServiceInConstructorParam>(resolveConstructorDependencies: true);
            container.Register<IService, ServiceImplementationParameteredConstructor>(resolveConstructorDependencies: true);

            // Assert
            var service1 = container!.Resolve<IService>();

            Assert.IsType<ServiceImplementationParameteredConstructor>(service1);
            Assert.IsType<ServiceInConstructorParam>(service1.Param);
            Assert.NotNull(service1);
            Assert.NotNull(service1.Param);
        }

        [Fact]
        public void Register_Dont_Resolve_Constructor_Params_Success()
        {
            // Arrange
            var container = TSwiftIoC.Instance;

            // Act
            container!.Register<IServiceInConstructorParam, ServiceInConstructorParam>();
            container.Register<IService, ServiceImplementationParameteredConstructor>();

            // Assert
            var service1 = container!.Resolve<IService>();

            Assert.IsType<ServiceImplementationParameteredConstructor>(service1);
            Assert.NotNull(service1);
            Assert.Null(service1.Param);
        }
    }  
}
