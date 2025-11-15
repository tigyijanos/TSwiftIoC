using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using TSwiftIoC.Enums;

namespace TSwiftIoC.Benchmarks
{
    public interface ITestService { }
    public class TestService : ITestService { }
    
    public interface IComplexService 
    { 
        ITestService? Dependency { get; }
    }
    
    public class ComplexService : IComplexService
    {
        public ITestService? Dependency { get; }
        public ComplexService(ITestService dependency)
        {
            Dependency = dependency;
        }
        public ComplexService() { }
    }

    [MemoryDiagnoser]
    [SimpleJob(warmupCount: 3, iterationCount: 5)]
    public class IoCBenchmarks
    {
        [GlobalSetup]
        public void Setup()
        {
            // Reset container for each benchmark
            TSwiftContainer.Instance = null;
        }

        [Benchmark]
        public void Register_Singleton()
        {
            var container = TSwiftContainer.Instance;
            for (int i = 0; i < 100; i++)
            {
                container!.Register<ITestService, TestService>($"key_{i}", Lifetime.Singleton);
            }
        }

        [Benchmark]
        public void Register_And_Resolve_Singleton()
        {
            var container = TSwiftContainer.Instance;
            container!.Register<ITestService, TestService>(lifetime: Lifetime.Singleton);
            
            for (int i = 0; i < 1000; i++)
            {
                var service = container.Resolve<ITestService>();
            }
        }

        [Benchmark]
        public void Register_And_Resolve_PerRequest()
        {
            var container = TSwiftContainer.Instance;
            container!.Register<ITestService, TestService>(lifetime: Lifetime.PerRequest);
            
            for (int i = 0; i < 1000; i++)
            {
                var service = container.Resolve<ITestService>();
            }
        }

        [Benchmark]
        public void Resolve_With_Dependencies()
        {
            var container = TSwiftContainer.Instance;
            container!.Register<ITestService, TestService>(lifetime: Lifetime.Singleton);
            container.Register<IComplexService, ComplexService>(lifetime: Lifetime.PerRequest, resolveConstructorDependencies: true);
            
            for (int i = 0; i < 1000; i++)
            {
                var service = container.Resolve<IComplexService>();
            }
        }

        [Benchmark]
        public void Register_Many_Services_With_Keys()
        {
            var container = TSwiftContainer.Instance;
            
            for (int i = 0; i < 1000; i++)
            {
                container!.Register<ITestService, TestService>($"service_{i}");
            }
        }

        [Benchmark]
        public void Resolve_Many_Keyed_Services()
        {
            var container = TSwiftContainer.Instance;
            
            // Setup
            for (int i = 0; i < 100; i++)
            {
                container!.Register<ITestService, TestService>($"service_{i}");
            }
            
            // Benchmark resolution
            for (int i = 0; i < 100; i++)
            {
                var service = container!.Resolve<ITestService>($"service_{i}");
            }
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<IoCBenchmarks>();
        }
    }
}
