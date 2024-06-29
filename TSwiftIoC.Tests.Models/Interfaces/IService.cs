namespace TSwiftIoC.Tests.Models.Interfaces
{
    public interface IService
    {
        Guid Guid { get; set; }

        IServiceInConstructorParam? Param { get; set; }
    }
}
