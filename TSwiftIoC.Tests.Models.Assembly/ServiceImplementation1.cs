using TSwiftIoC.Tests.Models.Interfaces.Assembly;

namespace TSwiftIoC.Tests.Models.Assembly
{
    public class ServiceImplementation1 : IService1
    {
        public Guid Guid { get; set; }

        public ServiceImplementation1()
        {
            Guid = Guid.NewGuid();
        }
    }
}
