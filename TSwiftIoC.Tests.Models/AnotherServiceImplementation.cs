using TSwiftIoC.Tests.Models.Interfaces;

namespace TSwiftIoC.Tests.Models
{
    public class AnotherServiceImplementation : IService
    {
        public Guid Guid { get; set; }
        public IServiceInConstructorParam? Param { get; set; }

        public AnotherServiceImplementation()
        {
            Guid = Guid.NewGuid();
        }
    }
}
