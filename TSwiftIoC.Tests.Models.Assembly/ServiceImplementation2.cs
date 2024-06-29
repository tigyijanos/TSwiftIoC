using TSwiftIoC.Tests.Models.Interfaces.Assembly;

namespace TSwiftIoC.Tests.Models.Assembly
{
    public class ServiceImplementation2 : IService2
    {
        public Guid Guid { get; set; }

        public ServiceImplementation2()
        {
            Guid = Guid.NewGuid();
        }
    }
}
