using TSwiftIoC.Tests.Models.Interfaces;

namespace TSwiftIoC.Tests.Models
{
    public class ServiceInConstructorParam : IServiceInConstructorParam
    {
        public Guid Guid { get; set; }

        public ServiceInConstructorParam()
        {
            Guid = Guid.NewGuid();
        }
    }
}
