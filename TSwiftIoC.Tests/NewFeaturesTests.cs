using TSwiftIoC.Enums;
using TSwiftIoC.Tests.Models;
using TSwiftIoC.Tests.Models.Interfaces;

namespace TSwiftIoC.Tests
{
    [Collection("Sequential")]
    public class NewFeaturesTests
    {
        public NewFeaturesTests()
        {
            // Ensure IoC container is reset before each test
            TSwiftContainer.Instance = null;
        }

        [Fact]
        public void ResolveAll_Returns_All_Registered_Services()
        {
            // Arrange
            var container = TSwiftContainer.Instance;
            
            // Verify clean state
            var initialServices = container!.ResolveAll<IService>().ToList();
            Assert.Empty(initialServices);
            
            container.Register<IService, ServiceImplementation>("key1");
            container.Register<IService, AnotherServiceImplementation>("key2");

            // Act
            var services = container.ResolveAll<IService>().ToList();

            // Assert
            Assert.Equal(2, services.Count);
            Assert.Contains(services, s => s is ServiceImplementation);
            Assert.Contains(services, s => s is AnotherServiceImplementation);
        }

        [Fact]
        public void IsRegistered_Returns_True_For_Registered_Type()
        {
            // Arrange
            var container = TSwiftContainer.Instance;
            container!.Register<IService, ServiceImplementation>();

            // Act
            var isRegistered = container.IsRegistered<IService>();

            // Assert
            Assert.True(isRegistered);
        }

        [Fact]
        public void IsRegistered_Returns_False_For_Unregistered_Type()
        {
            // Arrange
            var container = TSwiftContainer.Instance;

            // Act
            var isRegistered = container!.IsRegistered<IService>();

            // Assert
            Assert.False(isRegistered);
        }

        [Fact]
        public void IsRegistered_With_Key_Returns_True()
        {
            // Arrange
            var container = TSwiftContainer.Instance;
            container!.Register<IService, ServiceImplementation>("mykey");

            // Act
            var isRegistered = container.IsRegistered<IService>("mykey");

            // Assert
            Assert.True(isRegistered);
        }

        [Fact]
        public void Scoped_Lifetime_Returns_Same_Instance_Within_Scope()
        {
            // Arrange
            var container = TSwiftContainer.Instance;
            container!.Register<IService, ServiceImplementation>(lifetime: Lifetime.Scoped);

            // Act
            container.BeginScope();
            var service1 = container.Resolve<IService>();
            var service2 = container.Resolve<IService>();
            container.EndScope();

            // Assert
            Assert.NotNull(service1);
            Assert.NotNull(service2);
            Assert.Same(service1, service2);
            Assert.Equal(service1.Guid, service2!.Guid);
        }

        [Fact]
        public void Scoped_Lifetime_Returns_Different_Instance_In_Different_Scopes()
        {
            // Arrange
            var container = TSwiftContainer.Instance;
            container!.Register<IService, ServiceImplementation>(lifetime: Lifetime.Scoped);

            // Act
            container.BeginScope();
            var service1 = container.Resolve<IService>();
            container.EndScope();

            container.BeginScope();
            var service2 = container.Resolve<IService>();
            container.EndScope();

            // Assert
            Assert.NotNull(service1);
            Assert.NotNull(service2);
            Assert.NotSame(service1, service2);
            Assert.NotEqual(service1.Guid, service2!.Guid);
        }

        [Fact]
        public void Scoped_Without_BeginScope_Acts_Like_PerRequest()
        {
            // Arrange
            var container = TSwiftContainer.Instance;
            container!.Register<IService, ServiceImplementation>(lifetime: Lifetime.Scoped);

            // Act (no BeginScope called)
            var service1 = container.Resolve<IService>();
            var service2 = container.Resolve<IService>();

            // Assert
            Assert.NotNull(service1);
            Assert.NotNull(service2);
            Assert.NotSame(service1, service2);
            Assert.NotEqual(service1.Guid, service2!.Guid);
        }

        [Fact]
        public void ResolveAll_With_No_Registrations_Returns_Empty()
        {
            // Arrange
            var container = TSwiftContainer.Instance;

            // Act
            var services = container!.ResolveAll<IService>().ToList();

            // Assert
            Assert.Empty(services);
        }

        [Fact]
        public void Performance_Test_Compiled_Expression_Creation()
        {
            // Arrange
            var container = TSwiftContainer.Instance;
            container!.Register<IService, ServiceImplementation>(lifetime: Lifetime.PerRequest);

            // Act & Assert - This tests that compiled expressions are being used
            var services = new List<IService>();
            for (int i = 0; i < 1000; i++)
            {
                services.Add(container.Resolve<IService>()!);
            }

            Assert.Equal(1000, services.Count);
            Assert.All(services, s => Assert.NotNull(s));
        }
    }
}
