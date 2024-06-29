using TSwiftIoC.Tests.Models.Interfaces;

namespace TSwiftIoC.Tests.Models
{
    public class ServiceImplementation : IService
    {
        public Guid Guid { get; set; }
        public IServiceInConstructorParam? Param { get; set; }

        public ServiceImplementation()
        {
            Guid = Guid.NewGuid();
        }
    }
}
