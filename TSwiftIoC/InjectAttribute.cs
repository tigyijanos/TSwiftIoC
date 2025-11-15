namespace TSwiftIoC
{
    /// <summary>
    /// Attribute to mark properties for dependency injection
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class InjectAttribute : Attribute
    {
        public string? Key { get; set; }
    }
}
