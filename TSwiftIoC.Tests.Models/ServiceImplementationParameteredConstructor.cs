using TSwiftIoC.Tests.Models.Interfaces;

namespace TSwiftIoC.Tests.Models
{
    public class ServiceImplementationParameteredConstructor : IService
    {
        public Guid Guid { get; set; }
        public IServiceInConstructorParam? Param { get; set; }

        public ServiceImplementationParameteredConstructor()
        {
        }

        public ServiceImplementationParameteredConstructor(IServiceInConstructorParam param)
        {
            Param = param;
        }
    }
}
